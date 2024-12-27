namespace SimpleAzureQueueConsumer;

internal class QueueConfiguration
{
    internal string? QueueName { get; set; }

    /// <summary>
    /// Setting the polling rate in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="pollingRateMs"></param>
    internal int PollingRateMs { get; set; }

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
    internal string ErrorQueueName { get; set; } = "error";
    internal bool UseErrorQueue { get; set; }
    
    internal string GetQueueName()
    {
        return QueueName ?? throw new InvalidOperationException("Queue name not set");
    }
    
    internal string GetErrorQueueName()
    {
        if(QueueName is null)
        {
            throw new InvalidOperationException("Queue name not set");
        }
        
        return $"{QueueName}-{ErrorQueueName}";
    }
}