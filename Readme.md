# SimpleAzureQueueConsumer

## Description
SimpleAzureQueueConsumer is a .NET library that simplifies working with Azure Storage Queues. It provides an easy way to register message handlers with support for singleton, scoped, and transient lifecycles, along with a robust message dispatching mechanism.

It is a dead simple implementation, which uses a background service to poll messages from the Azure Storage Queue and dispatches them to the registered handlers. The library is designed to be lightweight and customizable, allowing developers to focus on building their message-driven systems without worrying about the underlying complexities.

To keep it simple, it will always just forward the message body a string to the handler. If you need to work with more complex message types, you can deserialize the message body in the handler.

If an unhandled exception occurs in the handler, the message will be retried 5 times, and finally sent to an error queue.

This package is designed for developers building message-driven systems using Azure Storage Queues.

---

## Features
- Keyed service registration for different queues.
- Built-in polling mechanism with configurable intervals.
- Easy integration with .NET's dependency injection (DI).
- Lightweight and customizable.

---

## Getting Started

### Installation
You can install the package using the .NET CLI:

```bash
dotnet add package SimpleAzureQueueConsumer
```
```bash
<PackageReference Include="SimpleAzureQueueConsumer" Version="1.0.0" />
```

### Usage
Here's a simple example of how you can use the `SimpleAzureQueueConsumer` library:

```csharp
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add base configuration
services.AddSaqcBase("YourAzureStorageConnectionString");

// Register handlers using the fluent API
builder.Services.AddSaqcHandler<OrderCreatedMessageHandler>()
    .OnQueue("order-created")
    .SetPollingRate(5000)
    .Register();

builder.Services.AddSaqcHandler<UserCreatedMessageHandler>()
    .OnQueue("user-created")
    .SetPollingRate(5000)
    .Register();

var app = builder.Build();
app.Run();
```

Create a new class that implements the `IQueueMessageHandler` interface:

```csharp
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
```

### Requirements
- .NET 6.0 or later (We are using keyed services, which are available in .NET 6.0 and later).
- Azure Storage Account with a Queue service.