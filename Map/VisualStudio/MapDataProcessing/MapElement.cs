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
        private readonly String _id;
        private readonly MapData _mapData;
        private readonly ElementName _name;
        private readonly ElementName _shortName;
        private readonly double _importance;
        private readonly List<int> _lookIds = new List<int>();
        private readonly int _categoryId;

        private readonly List<MapElement> _linkedElements1 = new List<MapElement>();
        private readonly List<MapElement> _linkedElements2 = new List<MapElement>();
        internal List<MapElement> LinkedElements1 { get { return _linkedElements1; } }
        internal List<MapElement> LinkedElements2 { get { return _linkedElements2; } }

        internal MapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId)
        {
            _id = id;
            _mapData = mapData;
            _name = new ElementName(name);
            _shortName = new ElementName(shortName);
            _importance = importance;
            _categoryId = mapData.getCategory(categoryId).Id;

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

            BsonArray _linkedElements1Array = new BsonArray();
            foreach (MapElement linkedElement in _linkedElements1) _linkedElements1Array.Add(linkedElement.Id);

            BsonArray _linkedElements2Array = new BsonArray();
            foreach (MapElement linkedElement in _linkedElements2) _linkedElements2Array.Add(linkedElement.Id);

            BsonDocument elementDocument = new BsonDocument()
            {
                { "map", _mapData.XmlMapData.parameters.mapId },
                { "id", _id},
                { "name", _name.getBsonDocument() },
                { "short_name", _shortName.getBsonDocument() },
                { "importance", _importance },
                { "linked_elements1", _linkedElements1Array },
                { "linked_elements2", _linkedElements2Array },
                { "look_ids", lookIdArray },
                { "category_id", _categoryId }
            };

            return elementDocument;
        }
    }
}
