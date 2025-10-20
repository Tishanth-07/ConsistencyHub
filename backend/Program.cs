using backend.Models;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind MongoDb settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddSingleton<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>());
#pragma warning restore CS8604 // Possible null reference argument.

// Register your services
builder.Services.AddSingleton<TestService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();


