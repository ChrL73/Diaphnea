using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PointMapElement : MapElement
    {
        private KmlFileData _KmlFile = null;

        internal PointMapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, string lookId) :
            base(id, mapData, name, shortName, lookId) { }

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
                if (_KmlFile != null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add several files ('{0}' and '{1}') to point element '{2}'", _KmlFile.Path, path, Id));
                    return -1;
                }
                else
                {
                    _KmlFile = data;
                }
            }

            return 0;
        }

        internal override int formParts()
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
                { "point", _KmlFile.PointList[0].getBsonDocument(MapData.XmlMapData.parameters.projection) }
            });

            pointElementCollection.InsertOne(elementDocument);

            return 0;
        }
    }
}
