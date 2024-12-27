using SimpleAzureQueueConsumer;
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
builder.AddSaqcHandler<OrderCreatedMessageHandler>("order-created");
builder.AddSaqcHandler<UserCreatedMessageHandler>("user-created");

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
    _users.Add(user);
    await sender.Send(user, "user-created");
    // await bus.Publish(new UserCreatedEvent(user.Email));
    return Results.Ok("User created");
});

app.Run();