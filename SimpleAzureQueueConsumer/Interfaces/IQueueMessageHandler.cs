using SimpleAzureQueueConsumer.Models;

namespace SimpleAzureQueueConsumer.Interfaces;

public interface IQueueMessageHandler
{
    /// <summary>
    /// Handles a message from the queue.<br/>
    /// If an exception is thrown, the message will be moved to the poison queue, if configured in queue configuration.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task HandleMessageAsync(StorageQueueMessage message);
}