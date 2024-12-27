using System.Text.Json;
using SimpleAzureQueueConsumer;

namespace WebApplication4.Consumers;

// Example message handler
public class OrderCreatedMessageHandler : IQueueMessageHandler
{
    public Task HandleMessageAsync(string message)
    {
        Console.WriteLine($"Order created message received: {message}");
        // Process the order created message
        var order = JsonSerializer.Deserialize<Order>(message);
        Console.WriteLine(order?.UserId);
        // ... your order processing logic ...
        return Task.CompletedTask;
    }
}