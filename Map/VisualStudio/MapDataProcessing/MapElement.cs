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

        internal MapElement(String id, MapData mapData)
        {
            _id = id;
            _mapData = mapData;
        }

        abstract internal int addKmlFile(String path);
        abstract internal int formParts();
        abstract internal int fillDatabase(IMongoDatabase database);

        protected String Id { get { return _id; } }
        protected MapData MapData { get { return _mapData; } }
    }
}
