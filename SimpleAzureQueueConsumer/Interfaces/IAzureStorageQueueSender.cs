namespace SimpleAzureQueueConsumer.Interfaces;

public interface IAzureStorageQueueSender
{
    public Task<bool> Send<T>(T message, string queueName);
}