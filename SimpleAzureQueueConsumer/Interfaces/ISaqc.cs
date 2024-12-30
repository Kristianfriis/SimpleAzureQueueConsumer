using System.Collections.Concurrent;
using Azure.Storage.Queues;

namespace SimpleAzureQueueConsumer.Interfaces;

internal interface ISaqc
{
    string GetConnectionString();
    void AddQueueName(string queueName, int pollingRateMs);
    List<IQueueConfiguration> GetQueueConfigurations();
    void ClearQueueNames();
    Task<QueueClient> GetOrCreateQueueClient(string queueName);
}