using SimpleAzureQueueConsumer.Interfaces;

namespace SimpleAzureQueueConsumer.Models;

internal class QueueConfiguration : IQueueConfiguration
{
    public string? QueueName { get; set; }

    /// <summary>
    /// Setting the polling rate in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="pollingRateMs"></param>
    public int PollingRateMs { get; set; }

    /// <summary>
    /// Setting the visibility timeout for a message when dequeued in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="visibilityTimeoutMs"></param>
    public int VisibilityTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Setting the dequeue count, before sending message to error queue<br />
    /// Default is 5
    /// </summary>
    /// <param name="dequeueCount">Integer setting the dequeue count. Default is 5</param>
    public int DequeueCount { get; set; } = 5;
    
    /// <summary>
    /// Setting the error queue name,   <br />
    /// If the error queue does not exist, it will be created. <br />
    /// </summary>
    /// <param name="errorQueueName">String setting the error queue name to append to queue-name. Default is "error"</param>
    public string ErrorQueueName { get; set; } = "error";

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
        
        return $"{QueueName}-{ErrorQueueName}";
    }

    public TimeSpan GetVisibilityTimeout()
    {
        return TimeSpan.FromMilliseconds(VisibilityTimeoutMs);
    }
}