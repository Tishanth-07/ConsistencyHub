using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ConsistencyHub.Services
{
    public class MongoContext : IMongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoContext(IConfiguration configuration)
        {
            var conn = configuration.GetSection("MongoDb:ConnectionString").Value;
            var dbName = configuration.GetSection("MongoDb:Database").Value ?? "consistencyhub";
            var client = new MongoClient(conn);
            Database = client.GetDatabase(dbName);
        }
    }
}
