using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ConsistencyHub.Services
{

    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IConfiguration configuration)
        {
            // Read MongoDbSettings section
            var mongoSettings = configuration.GetSection("MongoDbSettings");
            var connectionString = mongoSettings.GetValue<string>("ConnectionString");
            var databaseName = mongoSettings.GetValue<string>("DatabaseName");

            if (string.IsNullOrEmpty(connectionString))
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is missing in appsettings.json");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            if (string.IsNullOrEmpty(databaseName))
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException(nameof(databaseName), "MongoDB database name is missing in appsettings.json");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase Database => _database;
    }
}
