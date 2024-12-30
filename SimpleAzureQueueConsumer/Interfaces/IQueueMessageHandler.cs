namespace SimpleAzureQueueConsumer.Interfaces;

public interface IQueueMessageHandler
{
    /// <summary>
    /// Handles a message from the queue.<br/>
    /// If an exception is thrown, the message will be moved to the poison queue. By default, this will be the same queue with "-error" appended to the end.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task HandleMessageAsync(StorageQueueMessage message);
}