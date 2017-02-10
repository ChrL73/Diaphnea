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
        private readonly Dictionary<string, Look> _lookDictionary = new Dictionary<string, Look>();
        private readonly List<Look> _lookList = new List<Look>();

        private readonly Dictionary<XmlResolution, LineSuperposer> _lineSuperposerDictionary = new Dictionary<XmlResolution, LineSuperposer>();

        internal MapData(string dataFileName, XmlMapData xmlMapData)
        {
            _dataFileName = dataFileName;
            _xmlMapData = xmlMapData;
            foreach (XmlPolygonLook1 polygonLook1 in _xmlMapData.lookList.polygonLookList1)
            {
                Look look = new PolygonLook(polygonLook1);
                _lookDictionary.Add(polygonLook1.id.ToString(), look);
                _lookList.Add(look);
            }

            foreach (XmlPolygonLook2 polygonLook2 in _xmlMapData.lookList.polygonLookList2)
            {
                Look look = new PolygonLook(polygonLook2);
                _lookDictionary.Add(polygonLook2.id.ToString(), look);
                _lookList.Add(look);
            }

            foreach (XmlLineLook lineLook in _xmlMapData.lookList.lineLookList)
            {
                Look look = new LineLook(lineLook);
                _lookDictionary.Add(lineLook.id.ToString(), look);
                _lookList.Add(look);
            }

            foreach (XmlPointLook pointLook in _xmlMapData.lookList.pointLookList)
            {
                Look look = new PointLook(pointLook);
                _lookDictionary.Add(pointLook.id.ToString(), look);
                _lookList.Add(look);
            }

            foreach (XmlResolution xmlResolution in _xmlMapData.resolutionList) _lineSuperposerDictionary.Add(xmlResolution, new LineSuperposer());
        }

        internal XmlMapData XmlMapData
        {
            get { return _xmlMapData; }
        }

        internal string DataFileName
        {
            get { return _dataFileName; }
        }

        internal Look getLook(string id)
        {
            Look look;
            if (_lookDictionary.TryGetValue(id, out look)) return look;
            return null;
        }

        internal List<Look> LookList { get { return _lookList; } }

        internal  Dictionary<XmlResolution, LineSuperposer> LineSuperposerDictionary { get { return _lineSuperposerDictionary; } }
    }
}
