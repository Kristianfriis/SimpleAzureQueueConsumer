using System.Collections.Concurrent;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using SimpleAzureQueueConsumer.Interfaces;

namespace SimpleAzureQueueConsumer;

public class AzureStorageQueueSender : IAzureStorageQueueSender
{
    private SaqcOptions _options;
    private ConcurrentDictionary<string, QueueClient>? QueueClients { get; }
    
    public AzureStorageQueueSender(IOptions<SaqcOptions> options)
    {
        _options = options.Value;
        QueueClients = new ();
    }
    
    public async Task<bool> Send<T>(T message, string queueName)
    {
        var queueClient = await GetOrCreateQueueClient(queueName);
        
        var json = JsonSerializer.Serialize(message);

        try
        {
            await queueClient.SendMessageAsync(json);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task<QueueClient> GetOrCreateQueueClient(string queueName)
    {
        if(QueueClients is null)
        {
            //TODO User defined exception
            throw new InvalidOperationException("Queue clients not initialized.");
        }
        
        if (QueueClients.TryGetValue(queueName, out var queueClient))
        {
            return queueClient;
        }

        queueClient = new QueueClient(_options.ConnectionString, queueName);
        await queueClient.CreateIfNotExistsAsync();
        QueueClients.TryAdd(queueName, queueClient);
        return queueClient;
    }
}