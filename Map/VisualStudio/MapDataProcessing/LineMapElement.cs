using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LineMapElement : MapElement
    {
        private readonly List<KmlFileData> _KmlFileList = new List<KmlFileData>();

        internal LineMapElement(String id, MapData mapData) : base(id, mapData) { }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);

            if (data.Type == KmlFileTypeEnum.POINT)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add point file '{0}' to line element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.POLYGON)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add polygon file '{0}' to line element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.LINE)
            {
                _KmlFileList.Add(data);
            }

            return 0;
        }

        internal override int formParts()
        {
            return 0;
        }

        internal override int fillDatabase(IMongoDatabase database)
        {
            return 0;
        }
    }
}
