﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    abstract class MapElement
    {
        private readonly String _id;
        protected readonly MapData _mapData;
        private readonly ElementName _name;
        private readonly ElementName _shortName;
        private readonly double _importance;
        private readonly List<Look> _looks = new List<Look>();
        private readonly int _categoryId;

        private readonly Dictionary<MapElement, int> _linkedElements1 = new Dictionary<MapElement, int>();
        private readonly Dictionary<MapElement, int> _linkedElements2 = new Dictionary<MapElement, int>();
        internal Dictionary<MapElement, int> LinkedElements1 { get { return _linkedElements1; } }
        internal Dictionary<MapElement, int> LinkedElements2 { get { return _linkedElements2; } }

        internal List<Look> Looks { get { return _looks; } }

        internal MapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId)
        {
            _id = id;
            _mapData = mapData;
            _name = new ElementName(name);
            _shortName = new ElementName(shortName);
            _importance = importance;
            _categoryId = mapData.getCategory(categoryId).Id;

            foreach (string lookId in lookIds) _looks.Add(mapData.getLook(lookId));
        }

        abstract internal int addKmlFile(String path);
        abstract internal int formParts1();
        abstract internal int formParts2();
        abstract internal int fillDatabase(IMongoDatabase database);

        internal string Id { get { return _id; } }
        protected MapData MapData { get { return _mapData; } }
        internal int CategoryId { get { return _categoryId; } }

        protected BsonDocument getBsonDocument()
        {
            BsonArray lookIdArray = new BsonArray();
            foreach (Look look in _looks) lookIdArray.Add(look.Id);

            BsonArray _linkedElements1Array = new BsonArray();
            foreach (MapElement linkedElement in _linkedElements1.Keys) _linkedElements1Array.Add(linkedElement.Id);

            BsonArray _linkedElements2Array = new BsonArray();
            foreach (MapElement linkedElement in _linkedElements2.Keys) _linkedElements2Array.Add(linkedElement.Id);

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

        abstract internal int generateCode(CodeGenerator codeGenerator);
    }
}
