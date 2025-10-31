using MongoDB.Driver;

namespace ConsistencyHub.Services
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
    }
}
