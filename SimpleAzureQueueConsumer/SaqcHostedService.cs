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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task HandleTimerAsync(QueueConfiguration queueConfiguration, CancellationToken stoppingToken)
    {
        var queueClient = await SaqcBase.GetOrCreateQueueClient(queueConfiguration.QueueName);
        
        try
        {
            QueueMessage? message = await queueClient.ReceiveMessageAsync(TimeSpan.FromMinutes(5), stoppingToken);

            if (message != null)
            {
                if (message.DequeueCount > queueConfiguration.DequeueCount)
                {
                    await HandleError(queueConfiguration, message.MessageText, stoppingToken);
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
                        
                        await scopedHandler.HandleMessageAsync(message.MessageText); 
                    }
                    
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await HandleError(queueConfiguration, ex.Message, stoppingToken);
        }
    }

    private async Task StartQueueClients()
    {
        Console.WriteLine($"Creating queue clients for {SaqcBase.GetQueueConfigurations().Count()} queues");
        foreach (var queueConfiguration in SaqcBase.GetQueueConfigurations())
        {
            await SaqcBase.GetOrCreateQueueClient(queueConfiguration.QueueName);
            Console.WriteLine($"Queue client created for queue: {queueConfiguration.QueueName}");
        }
    }

    private async Task HandleError(QueueConfiguration queueConfiguration, string messageBody, CancellationToken stoppingToken)
    {
        var errorQueueName = $"{queueConfiguration}-error";
        var errorQueueClient = await SaqcBase.GetOrCreateQueueClient(errorQueueName);;
        var errorQueueExist = await errorQueueClient.ExistsAsync(stoppingToken);
                        
        if (!errorQueueExist)
        {
            await errorQueueClient.CreateAsync(cancellationToken: stoppingToken);
        }
                        
        await errorQueueClient.SendMessageAsync(messageBody, stoppingToken);
    }
}