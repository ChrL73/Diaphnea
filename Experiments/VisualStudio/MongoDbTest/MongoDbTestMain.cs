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
        static void Main(string[] args)
        {
            MongoClient client = new MongoClient();
            IMongoDatabase database = client.GetDatabase("testDb");

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("persons");
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
                {"road", "2 rue du sud"},
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
