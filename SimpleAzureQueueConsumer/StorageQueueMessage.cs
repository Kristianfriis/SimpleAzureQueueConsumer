using Azure.Storage.Queues.Models;

namespace SimpleAzureQueueConsumer;

public class StorageQueueMessage
{
    public string? MessageId { get; internal set; }
    public string? PopReceipt { get; internal set; }
    public string? MessageText { get; internal set; }
    public BinaryData? Body { get; internal set; }
    public long DequeueCount { get; internal set; }

    public StorageQueueMessage(QueueMessage queueMessage)
    {
        MessageId = queueMessage.MessageId;
        PopReceipt = queueMessage.PopReceipt;
        MessageText = queueMessage.MessageText;
        Body = queueMessage.Body;
        DequeueCount = queueMessage.DequeueCount;
    }
}