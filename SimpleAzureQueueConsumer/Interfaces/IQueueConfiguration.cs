namespace SimpleAzureQueueConsumer.Interfaces;

internal interface IQueueConfiguration
{
    string? QueueName { get; set; }
    int PollingRateMs { get; set; }
    int VisibilityTimeoutMs { get; set; }
    int DequeueCount { get; set; }
    string ErrorQueueName { get; set; }
    bool UseErrorQueue { get; set; }
    string GetQueueName();
    string GetErrorQueueName();
    TimeSpan GetVisibilityTimeout();
}