using System.Collections.Concurrent;
using Azure.Storage.Queues;

namespace SimpleAzureQueueConsumer;

internal static class SaqcBase
{
    private static string? ConnectString { get; set; }
    private static List<QueueConfiguration> QueueConfigs { get; } = new();
    private static readonly ConcurrentDictionary<string, QueueClient> QueueClients = new();

    internal static void SetConnectionString(string connectionString)
    {
        ConnectString = connectionString;
    }

    private static string GetConnectionString()
    {
        return ConnectString ?? throw new InvalidOperationException("Connection string not set");
    }

    internal static void AddQueueName(string queueName, int pollingRateMs)
    {
        if(QueueConfigs.Any(qc => qc.QueueName == queueName))
        {
            throw new InvalidOperationException($"Queue with name {queueName} already configured.");
        }
        
        var queueConfig = new QueueConfiguration
        {
            QueueName = queueName,
            PollingRateMs = pollingRateMs,
        };
        QueueConfigs.Add(queueConfig);
    }

    internal static void AddQueueConfig(QueueConfiguration queueConfiguration)
    {
        if(QueueConfigs.Any(qc => qc.QueueName == queueConfiguration.QueueName))
        {
            throw new InvalidOperationException($"Queue with name {queueConfiguration.QueueName} already configured.");
        }
        
        QueueConfigs.Add(queueConfiguration);
    }

    public static List<QueueConfiguration> GetQueueConfigurations()
    {
        return QueueConfigs;
    }

    internal static void ClearQueueNames()
    {
        QueueConfigs.Clear();
    }

    internal static async Task<QueueClient> GetOrCreateQueueClient(string queueName)
    {
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