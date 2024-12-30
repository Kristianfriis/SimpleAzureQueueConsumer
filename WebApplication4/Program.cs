using SimpleAzureQueueConsumer.Extensions;
using SimpleAzureQueueConsumer.Interfaces;
using WebApplication4;
using WebApplication4.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString =
    "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

builder.AddSaqc(connString);
builder.AddSaqcHandler<UserCreatedMessageHandler>("user-created");

builder.AddSaqcHandler<OrderCreatedMessageHandler>()
    .OnQueue("order-created")
    .WithVisibilityTimeout(new TimeSpan(0,0,0,1))
    .WithPollingInterval(1000)
    .Register();

// builder.AddSaqcHandler<OrderCreatedMessageHandler>("order-created");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var _users = new List<User>();

app.MapPost("/api/CreateUser", async (User user, IAzureStorageQueueSender sender) =>
{
    if (_users.Any(u => string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest("User already exists");
    }
    user.Id = _users.Count + 1;
    _users.Add(user);
    await sender.Send(user, "user-created");
    // await bus.Publish(new UserCreatedEvent(user.Email));
    return Results.Ok("User created");
});

app.Run();