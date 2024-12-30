using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleAzureQueueConsumer;

internal class SaqcHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Task> _listenerTasks = new();
    private readonly SemaphoreSlim _semaphore;

    public SaqcHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _semaphore = new SemaphoreSlim(10);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting the Azure Queue Consumer Hosted Service");
        await StartQueueClients();
        foreach (var queueConfiguration in SaqcBase.GetQueueConfigurations())
        {
            _listenerTasks.Add(Task.Run(() => QueueTimer(queueConfiguration, stoppingToken), stoppingToken));
        }

        await Task.WhenAll(_listenerTasks);
    }

    private async Task QueueTimer(QueueConfiguration queueConfiguration, CancellationToken stoppingToken)
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

    private async Task HandleTimerAsync(QueueConfiguration queueConfiguration, CancellationToken stoppingToken)
    {
        var queueClient = await SaqcBase.GetOrCreateQueueClient(queueConfiguration.GetQueueName());

        QueueMessage? message = null;
        
        try
        {
            message = await queueClient.ReceiveMessageAsync(queueConfiguration.GetVisibilityTimeout(), stoppingToken);

            if (message != null)
            {
                if (message.DequeueCount > queueConfiguration.DequeueCount)
                {
                    Console.WriteLine("Message has been dequeued too many times. Sending to error queue.");
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
            Console.WriteLine(ex);
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
        Console.WriteLine($"Creating queue clients for {SaqcBase.GetQueueConfigurations().Count()} queues");
        foreach (var queueConfiguration in SaqcBase.GetQueueConfigurations())
        {
            await SaqcBase.GetOrCreateQueueClient(queueConfiguration.GetQueueName());
            Console.WriteLine($"Queue client created for queue: {queueConfiguration.QueueName}");
        }
    }

    private async Task HandleError(QueueConfiguration queueConfiguration, QueueMessage? message,  QueueClient? queueClient, CancellationToken stoppingToken)
    {
        if(message is null)
            return;
            
        var errorQueueClient = await SaqcBase.GetOrCreateQueueClient(queueConfiguration.GetErrorQueueName());
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
            Console.WriteLine("SaqcHostedService stopping gracefully.");
            await base.StopAsync(cancellationToken); 
        }
        catch (OperationCanceledException)
        {
            // Suppress the cancellation exception as it's expected during shutdown
            Console.WriteLine("SaqcHostedService: OperationCanceledException during StopAsync.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("SaqcHostedService: An error occurred during shutdown.");
        }
    }
}