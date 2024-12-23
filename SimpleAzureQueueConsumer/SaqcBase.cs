using System.Collections.Concurrent;
using Azure.Storage.Queues;

namespace SimpleAzureQueueConsumer;

internal static class SaqcBase
{
    private static string? ConnectString { get; set; }
    private static List<QueueConfiguration> QueueConfigs { get; } = new();
    private static readonly ConcurrentDictionary<string, QueueClient> QueueClients = new();

    public static void SetConnectionString(string connectionString)
    {
        ConnectString = connectionString;
    }

    public static string GetConnectionString()
    {
        return ConnectString ?? throw new InvalidOperationException("Connection string not set");
    }

    public static void AddQueueName(string queueName, int pollingRateMs)
    {
        var queueConfig = new QueueConfiguration
        {
            QueueName = queueName,
            PollingRateMs = pollingRateMs,
        };
        QueueConfigs.Add(queueConfig);
    }

    public static List<QueueConfiguration> GetQueueConfigurations()
    {
        return QueueConfigs;
    }

    public static void ClearQueueNames()
    {
        QueueConfigs.Clear();
    }

    internal static async Task<QueueClient> GetOrCreateQueueClient(string queueName)
    {
        if (QueueClients.TryGetValue(queueName, out var queueClient))
        {
            return queueClient;
        }

        queueClient = new QueueClient(SaqcBase.GetConnectionString(), queueName);
        await queueClient.CreateIfNotExistsAsync();
        QueueClients.TryAdd(queueName, queueClient);
        return queueClient;
    }
}