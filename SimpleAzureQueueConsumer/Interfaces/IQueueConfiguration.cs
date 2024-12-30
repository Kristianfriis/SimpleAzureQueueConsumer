namespace SimpleAzureQueueConsumer;

internal interface IQueueConfiguration
{
    string? QueueName { get; set; }

    /// <summary>
    /// Setting the polling rate in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="pollingRateMs"></param>
    int PollingRateMs { get; set; }

    /// <summary>
    /// Setting the visibility timeout for a message when dequeued in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="visibilityTimeoutMs"></param>
    int VisibilityTimeoutMs { get; set; }

    /// <summary>
    /// Setting the dequeue count, before sending message to error queue<br />
    /// Default is 5
    /// </summary>
    /// <param name="dequeueCount">Integer setting the dequeue count. Default is 5</param>
    int DequeueCount { get; set; }

    /// <summary>
    /// Setting the error queue name,   <br />
    /// If the error queue does not exist, it will be created. <br />
    /// </summary>
    /// <param name="errorQueueName">String setting the error queue name to append to queue-name. Default is "error"</param>
    string ErrorQueueName { get; set; }

    bool UseErrorQueue { get; set; }
    string GetQueueName();
    string GetErrorQueueName();
    TimeSpan GetVisibilityTimeout();
}