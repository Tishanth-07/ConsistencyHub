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
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is missing in appsettings.json");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName), "MongoDB database name is missing in appsettings.json");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase Database => _database;
    }
}
