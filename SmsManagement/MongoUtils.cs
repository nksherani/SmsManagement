using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsManagement
{
    public static class MyMongoUtils
    {
        public static IMongoDatabase database = GetMongoDb();

        public static IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            return collection;
        }

        private static IMongoDatabase GetMongoDb()
        {
            // Set up MongoDB connection string and database/collection names
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "SMSDb";

            // Create MongoClient and get database and collection references
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            return database;
        }

        public static void CreateUniquenessIndex(IMongoCollection<BsonDocument> collection)
        {

            // Index keys
            var keys = Builders<BsonDocument>.IndexKeys.Ascending("address").Ascending("date").Ascending("body");

            // Index options (including unique constraint)
            var options = new CreateIndexOptions { Unique = true };

            // Create index
            collection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(keys, options));

            // Optionally, check if the index was created successfully
            var indexList = collection.Indexes.List().ToList();
            foreach (var index in indexList)
            {
                Console.WriteLine(index["name"]);
            }
        }
    }
}
