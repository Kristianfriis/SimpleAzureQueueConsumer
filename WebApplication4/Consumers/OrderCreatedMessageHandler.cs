using SimpleAzureQueueConsumer;

namespace WebApplication4.Consumers;

// Example message handler
public class OrderCreatedMessageHandler : IQueueMessageHandler
{
    public Task HandleMessageAsync(string message)
    {
        // Process the order created message
        Console.WriteLine($"Order created message received: {message}");
        // ... your order processing logic ...
        return Task.CompletedTask;
    }
}