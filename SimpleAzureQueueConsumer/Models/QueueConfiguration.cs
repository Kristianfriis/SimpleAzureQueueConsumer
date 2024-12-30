using SimpleAzureQueueConsumer.Interfaces;

namespace SimpleAzureQueueConsumer.Models;

internal class QueueConfiguration : IQueueConfiguration
{
    public string? QueueName { get; set; }
    public int PollingRateMs { get; set; } = 5000;
    public int VisibilityTimeoutMs { get; set; } = 300;
    public int DequeueCount { get; set; } = 5;
    public string ErrorQueueName { get; set; } = string.Empty;
    public bool UseErrorQueue { get; set; }

    public string GetQueueName()
    {
        return QueueName ?? throw new InvalidOperationException("Queue name not set");
    }

    public string GetErrorQueueName()
    {
        if(QueueName is null)
        {
            throw new InvalidOperationException("Queue name not set");
        }

        if (!string.IsNullOrEmpty(ErrorQueueName))
            return ErrorQueueName;
                
        return $"{QueueName}-error";
    }

    public TimeSpan GetVisibilityTimeout()
    {
        return TimeSpan.FromMilliseconds(VisibilityTimeoutMs);
    }
}