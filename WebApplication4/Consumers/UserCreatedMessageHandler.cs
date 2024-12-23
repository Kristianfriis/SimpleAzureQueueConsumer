using System.Text.Json;
using SimpleAzureQueueConsumer;

namespace WebApplication4.Consumers;

public class UserCreatedMessageHandler : IQueueMessageHandler
{
    public Task HandleMessageAsync(string message)
    {
        var user = JsonSerializer.Deserialize<User>(message);
        Console.WriteLine($"User created message received: user id: {user?.Email}");
        // ... your order processing logic ...
        return Task.CompletedTask;
    }
}