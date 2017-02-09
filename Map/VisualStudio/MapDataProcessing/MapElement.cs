using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    abstract class MapElement
    {
        private static int _counter = -1;
        internal static void reset() { _counter = -1; }

        private readonly int _numericalId;
        private readonly String _id;
        private readonly MapData _mapData;
        private readonly ElementName _name;
        private readonly ElementName _shortName;
        private readonly double _importance;
        private readonly List<int> _lookIds = new List<int>();

        internal MapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds)
        {
            _numericalId = ++_counter;
            _id = id;
            _mapData = mapData;
            _name = new ElementName(name);
            _shortName = new ElementName(shortName);
            _importance = importance;

            foreach (string lookId in lookIds) _lookIds.Add(mapData.getLook(lookId).Id);
        }

        abstract internal int addKmlFile(String path);
        abstract internal int formParts1();
        abstract internal int formParts2();
        abstract internal int fillDatabase(IMongoDatabase database);

        protected String Id { get { return _id; } }
        protected MapData MapData { get { return _mapData; } }

        protected BsonDocument getBsonDocument()
        {
            BsonArray lookIdArray = new BsonArray();
            foreach (int lookId in _lookIds) lookIdArray.Add(lookId);

            BsonDocument elementDocument = new BsonDocument()
            {
                { "map", _mapData.XmlMapData.parameters.mapId },
                { "id", _id},
                //{ "num_id", _numericalId}, // No longer used
                { "name", _name.getBsonDocument() },
                { "short_name", _shortName.getBsonDocument() },
                { "importance", _importance },
                { "look_ids", lookIdArray }
            };

            return elementDocument;
        }
    }
}
