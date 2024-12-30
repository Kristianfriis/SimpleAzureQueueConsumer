using Azure.Storage.Queues.Models;

namespace SimpleAzureQueueConsumer.Models;

/// <summary>
/// Represents a message retrieved from an Azure Storage Queue.
/// </summary>
public class StorageQueueMessage
{
    /// <summary>
    /// Gets the unique identifier for the message.
    /// </summary>
    public string? MessageId { get; internal set; }

    /// <summary>
    /// Gets the pop receipt value for the message.
    /// </summary>
    public string? PopReceipt { get; internal set; }

    /// <summary>
    /// Gets the text of the message.
    /// </summary>
    public string? MessageText { get; internal set; }

    /// <summary>
    /// Gets the binary data of the message body.
    /// </summary>
    public BinaryData? Body { get; internal set; }

    /// <summary>
    /// Gets the number of times the message has been dequeued.
    /// </summary>
    public long DequeueCount { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageQueueMessage"/> class.
    /// </summary>
    /// <param name="queueMessage">The queue message from Azure Storage Queue.</param>
    public StorageQueueMessage(QueueMessage queueMessage)
    {
        MessageId = queueMessage.MessageId;
        PopReceipt = queueMessage.PopReceipt;
        MessageText = queueMessage.MessageText;
        Body = queueMessage.Body;
        DequeueCount = queueMessage.DequeueCount;
    }
}