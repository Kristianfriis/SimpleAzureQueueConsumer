using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleAzureQueueConsumer.Helpers;
using SimpleAzureQueueConsumer.Interfaces;
using SimpleAzureQueueConsumer.Models;

namespace SimpleAzureQueueConsumer;

internal class SaqcHostedService : BackgroundService
{
    private readonly ISaqc _saqc;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Task> _listenerTasks = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly SaqcOptions _options;

    public SaqcHostedService(IServiceProvider serviceProvider, ISaqc saqc, IOptions<SaqcOptions> options)
    {
        _serviceProvider = serviceProvider;
        _saqc = saqc;
        _semaphore = new SemaphoreSlim(options.Value.NumberOfWorkers);
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoggerHelper.LogInfo("Starting the Azure Queue Consumer Hosted Service", _serviceProvider, _options.LoggingEnabled);
        await StartQueueClients();
        foreach (var queueConfiguration in _saqc.GetQueueConfigurations())
        {
            _listenerTasks.Add(Task.Run(() => QueueTimer(queueConfiguration, stoppingToken), stoppingToken));
        }

        await Task.WhenAll(_listenerTasks);
    }

    private async Task QueueTimer(IQueueConfiguration queueConfiguration, CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(queueConfiguration.PollingRateMs));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await _semaphore.WaitAsync(stoppingToken);
                try
                {
                    await HandleTimerAsync(queueConfiguration, stoppingToken);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task HandleTimerAsync(IQueueConfiguration queueConfiguration, CancellationToken stoppingToken)
    {
        var queueClient = await _saqc.GetOrCreateQueueClient(queueConfiguration.GetQueueName());

        QueueMessage? message = null;
        
        try
        {
            message = await queueClient.ReceiveMessageAsync(queueConfiguration.GetVisibilityTimeout(), stoppingToken);

            if (message != null)
            {
                if (message.DequeueCount > queueConfiguration.DequeueCount)
                {
                    LoggerHelper.LogWarning("Message has been dequeued too many times. Sending to error queue.", _serviceProvider, _options.LoggingEnabled);
                    await HandleError(queueConfiguration, message, queueClient, stoppingToken);
                }
                else
                {
                    using (var scope = _serviceProvider.CreateScope()) 
                    {
                        var scopedHandler = scope.ServiceProvider.GetRequiredKeyedService<IQueueMessageHandler>(queueConfiguration.QueueName); 
                        
                        if(scopedHandler is null)
                        {
                            throw new InvalidOperationException($"No handler found for queue: {queueConfiguration.QueueName}");
                        }
                        
                        var storageQueueMessage = new StorageQueueMessage(message);
                        
                        await scopedHandler.HandleMessageAsync(storageQueueMessage); 
                    }
                    
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            var logMessage = "HandleTimerAsync: An error occurred during timer.";
            LoggerHelper.LogError(ex, logMessage, _serviceProvider, _options.LoggingEnabled);
            if (message is not null)
            {
                if (message.DequeueCount < queueConfiguration.DequeueCount)
                    return;
                
                await HandleError(queueConfiguration, message, queueClient, stoppingToken);
            }
        }
    }

    private async Task StartQueueClients()
    {
        LoggerHelper.LogInfo($"Creating queue clients for {_saqc.GetQueueConfigurations().Count()} queues", _serviceProvider, _options.LoggingEnabled);
        foreach (var queueConfiguration in _saqc.GetQueueConfigurations())
        {
            await _saqc.GetOrCreateQueueClient(queueConfiguration.GetQueueName());
            LoggerHelper.LogInfo($"Queue client created for queue: {queueConfiguration.QueueName}", _serviceProvider, _options.LoggingEnabled);
        }
    }

    private async Task HandleError(IQueueConfiguration queueConfiguration, QueueMessage? message,  QueueClient? queueClient, CancellationToken stoppingToken)
    {
        if(message is null)
            return;
            
        var errorQueueClient = await _saqc.GetOrCreateQueueClient(queueConfiguration.GetErrorQueueName());
        var errorQueueExist = await errorQueueClient.ExistsAsync(stoppingToken);
                        
        if (!errorQueueExist)
        {
            await errorQueueClient.CreateAsync(cancellationToken: stoppingToken);
        }
                        
        await errorQueueClient.SendMessageAsync(message.MessageText, stoppingToken);
        
        if(queueClient is not null)
            //removing the original message from the queue
            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            LoggerHelper.LogError( "SaqcHostedService stopping gracefully.", _serviceProvider, _options.LoggingEnabled);
            await base.StopAsync(cancellationToken); 
        }
        catch (OperationCanceledException)
        {
            var logMessage = "SaqcHostedService: OperationCanceledException during StopAsync.";
            // Suppress the cancellation exception as it's expected during shutdown
            LoggerHelper.LogError(logMessage, _serviceProvider, _options.LoggingEnabled);
            
        }
        catch (Exception ex)
        {
            var logMessage = "SaqcHostedService: An error occurred during shutdown.";
            LoggerHelper.LogError(ex, logMessage, _serviceProvider, _options.LoggingEnabled);
        }
    }
}