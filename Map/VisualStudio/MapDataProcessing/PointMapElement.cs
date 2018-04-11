using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class PointMapElement : MapElement
    {
        private KmlFileData _kmlFile = null;
        private readonly ItemId _itemId = new ItemId();

        internal PointMapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId) :
            base(id, mapData, name, shortName, importance, lookIds, categoryId) { }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);

            if (data.Type == KmlFileTypeEnum.LINE)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add line file '{0}' to point element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.POLYGON)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add polygon file '{0}' to point element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.POINT)
            {
                if (_kmlFile != null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add several files ('{0}' and '{1}') to point element '{2}'", _kmlFile.Path, path, Id));
                    return -1;
                }
                else
                {
                    _kmlFile = data;
                }
            }

            return 0;
        }

        internal GeoPoint Point
        {
            get
            {
                return _kmlFile.PointList[0];
            }
        }

        internal override int formParts1()
        {
            if (_kmlFile == null)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("No KML file associated to element '{0}'", Id));
                return -1;
            }

            return 0;
        }

        internal override int formParts2()
        {
            return 0;
        }

        internal override int fillDatabase(IMongoDatabase database)
        {
            IMongoCollection<BsonDocument> pointElementCollection = database.GetCollection<BsonDocument>("point_elements");

            BsonDocument elementDocument = new BsonDocument();
            elementDocument.AddRange(getBsonDocument());
            elementDocument.AddRange(new BsonDocument()
            {
                { "item_id", _itemId.Value },
                { "point", _kmlFile.PointList[0].getBsonDocument(null, null, null, MapData.XmlMapData.parameters.projection) }
            });

            pointElementCollection.InsertOne(elementDocument);

            return 0;
        }
    }
}
