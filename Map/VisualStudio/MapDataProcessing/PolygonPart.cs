using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonPart
    {
        private static Dictionary<KmlFileData, PolygonPart> _partDictionary = new Dictionary<KmlFileData, PolygonPart>();

        internal static PolygonPart getPart(KmlFileData lineData, KmlFileData pointData1, KmlFileData pointData2)
        {
            PolygonPart part;
            if (!_partDictionary.TryGetValue(lineData, out part))
            {
                part = new PolygonPart(lineData, pointData1, pointData2);
                _partDictionary.Add(lineData, part);
            }
            else
            {
                if (pointData1 != part._pointData1)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Different point1s ('{0}' and '{1}') for line polygon part '{2}'",
                                             pointData1, part._pointData1, lineData));
                    return null;
                }
                else if (pointData2 != part._pointData2)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Different point2s ('{0}' and '{1}') for line polygon part '{2}'",
                                             pointData2, part._pointData2, lineData));
                    return null;
                }
            }

            return part;
        }

        private readonly KmlFileData _lineData;
        private readonly KmlFileData _pointData1;
        private readonly KmlFileData _pointData2;

        private PolygonPart(KmlFileData lineData, KmlFileData pointData1, KmlFileData pointData2)
        {
            _lineData = lineData;
            _pointData1 = pointData1;
            _pointData2 = pointData2;
        }
    }
}
