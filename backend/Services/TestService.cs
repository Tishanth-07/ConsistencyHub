using MongoDB.Driver;
using backend.Models;

namespace backend.Services
{
    public class TestService
    {
        private readonly IMongoCollection<Test> _tests;

        public TestService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _tests = database.GetCollection<Test>("Tests");
        }

        public async Task<List<Test>> GetAsync() =>
            await _tests.Find(t => true).ToListAsync();

        public async Task<Test> CreateAsync(Test test)
        {
            await _tests.InsertOneAsync(test);
            return test;
        }
    }
}
