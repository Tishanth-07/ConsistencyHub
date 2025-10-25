using ConsistencyHub.Services;
using ConsistencyHub.Models;

var builder = WebApplication.CreateBuilder(args);

// Bind MongoDB settings correctly
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register your service
builder.Services.AddSingleton<TestService>();

// Add other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
