using YourApp.Models;
using YourApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind MongoDb settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>());

// Register service
builder.Services.AddSingleton<TestService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
