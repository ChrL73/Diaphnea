using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    class MongoDbTestMain
    {
        private static IMongoDatabase _database;

        static void Main(string[] args)
        {
            MongoClient client = new MongoClient();
            _database = client.GetDatabase("testDb");

            insertionTest();
            createTestCollection();

        }

        static private void createTestCollection()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("test");
            BsonDocument filter = new BsonDocument();
            DeleteResult result = collection.DeleteMany(filter);

            uint i;
            for (i = 0; i < 100; ++i)
            {
                BsonDocument doc1 = new BsonDocument()
                {
                    { "stringField", stringField(i) }
                };
                collection.InsertOne(doc1);
            }       
        }

        static private string stringField(uint i)
        {
            string s = ((uint)(Math.Exp((Double)i)).GetHashCode()).ToString();
            int j;
            for (j = 0; j < 10; ++j) s = s.Replace(j.ToString()[0], (char)(j + 97));
            return s;
        }

        static private void insertionTest()
        {
            IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>("persons");
            BsonDocument filter = new BsonDocument();
            DeleteResult result = collection.DeleteMany(filter);

            BsonDocument doc1 = new BsonDocument()
            {
                {"firstName", "Michel"},
                {"name", "Martin"}
            };
            BsonElement elem1 = new BsonElement("address", "1 rue du Nord");
            doc1.Add(elem1);
            collection.InsertOne(doc1);

            BsonDocument doc2 = new BsonDocument()
            {
                {"road", "2 rue du Sud"},
                {"city", "Paris"}
            };
            BsonDocument doc3 = new BsonDocument()
            {
                {"firstName", "Bill"},
                {"name", "Blup"},
                {"address", doc2}
            };
            collection.InsertOne(doc3);
        }
    }
}
