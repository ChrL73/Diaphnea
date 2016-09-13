using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonMapElement : MapElement
    {
        private readonly List<KmlFileData> _pointKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _lineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _polygonKmlFileList = new List<KmlFileData>();
        
        internal PolygonMapElement(String id) : base(id) { }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);
            if (data.Type == KmlFileTypeEnum.POINT) _pointKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.LINE) _lineKmlFileList.Add(data);
            if (data.Type == KmlFileTypeEnum.POLYGON) _polygonKmlFileList.Add(data);

            return 0;
        }
    }
}
