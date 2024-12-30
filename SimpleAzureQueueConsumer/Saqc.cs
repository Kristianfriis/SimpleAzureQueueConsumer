using System.Collections.Concurrent;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using SimpleAzureQueueConsumer.Interfaces;

namespace SimpleAzureQueueConsumer;

internal class Saqc : ISaqc
{
    private IEnumerable<IQueueConfiguration>? QueueConfigs { get; }
    private ConcurrentDictionary<string, QueueClient>? QueueClients { get; }
    private readonly SaqcOptions _options;

    public Saqc(IOptions<SaqcOptions> options, IEnumerable<IQueueConfiguration> queueConfigs)
    {
        _options = options.Value;
        QueueConfigs = queueConfigs;
        QueueClients = new ();
    }

    public string GetConnectionString()
    {
        return _options.ConnectionString;
    }

    public void AddQueueName(string queueName, int pollingRateMs)
    {
        throw new NotImplementedException();
    }

    public List<IQueueConfiguration> GetQueueConfigurations()
    {
        if (QueueConfigs is null)
            return new List<IQueueConfiguration>();
        
        return QueueConfigs.ToList();
    }

    public void ClearQueueNames()
    {
        throw new NotImplementedException();
    }

    public async Task<QueueClient> GetOrCreateQueueClient(string queueName)
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

        queueClient = new QueueClient(GetConnectionString(), queueName);
        await queueClient.CreateIfNotExistsAsync();
        QueueClients.TryAdd(queueName, queueClient);
        return queueClient;
    }
}