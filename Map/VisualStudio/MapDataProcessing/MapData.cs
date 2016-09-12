using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class MapData
    {
        private readonly string _dataFileName;
        private readonly XmlMapData _xmlMapData;
        private readonly Dictionary<string, XmlPolygonLook> _polygonLookDictionary = new Dictionary<string, XmlPolygonLook>();
        private readonly Dictionary<string, XmlLineLook> _lineLookDictionary = new Dictionary<string, XmlLineLook>();
        private readonly Dictionary<string, XmlPointLook> _pointLookDictionary = new Dictionary<string, XmlPointLook>();

        internal MapData(string dataFileName, XmlMapData xmlMapData)
        {
            _dataFileName = dataFileName;
            _xmlMapData = xmlMapData;
            foreach (XmlPolygonLook look in _xmlMapData.lookList.polygonLookList) _polygonLookDictionary.Add(look.id.ToString(), look);
            foreach (XmlLineLook look in _xmlMapData.lookList.lineLookList) _lineLookDictionary.Add(look.id.ToString(), look);
            foreach (XmlPointLook look in _xmlMapData.lookList.pointLookList) _pointLookDictionary.Add(look.id.ToString(), look);
        }

        internal XmlMapData XmlMapData
        {
            get { return _xmlMapData; }
        }

        internal string DataFileName
        {
            get { return _dataFileName; }
        }

        internal XmlPolygonLook getXmlPolygonLook(string id)
        {
            XmlPolygonLook look;
            if (_polygonLookDictionary.TryGetValue(id, out look)) return look;
            return null;
        }

        internal XmlLineLook getXmlLineLook(string id)
        {
            XmlLineLook look;
            if (_lineLookDictionary.TryGetValue(id, out look)) return look;
            return null;
        }

        internal XmlPointLook getXmlPointLook(string id)
        {
            XmlPointLook look;
            if (_pointLookDictionary.TryGetValue(id, out look)) return look;
            return null;
        }
    }
}
