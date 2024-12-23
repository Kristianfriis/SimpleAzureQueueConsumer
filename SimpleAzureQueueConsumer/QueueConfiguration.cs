namespace SimpleAzureQueueConsumer;

internal class QueueConfiguration
{
    public required string QueueName { get; set; }

    /// <summary>
    /// Setting the polling rate in milliseconds.<br />
    ///
    /// Default is 5000ms
    /// </summary>
    /// <param name="pollingRateMs"></param>
    public int PollingRateMs { get; set; }

    /// <summary>
    /// Settoing the dequeue count, before sending message to error queue<br />
    /// Default is 5
    /// </summary>
    /// <param name="dequeueCount"></param>
    public int DequeueCount { get; set; } = 5;
}