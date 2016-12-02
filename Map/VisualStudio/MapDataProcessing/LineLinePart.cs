using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LineLinePart : IAttachment
    {
        private static Dictionary<KmlFileData, LineLinePart> _partDictionary = new Dictionary<KmlFileData, LineLinePart>();

        internal static void clearAll()
        {
            _partDictionary.Clear();
        }

        internal static bool exists(KmlFileData lineData)
        {
            return _partDictionary.ContainsKey(lineData);
        }

        internal static LineLinePart getPart(KmlFileData lineData)
        {
            if (PolygonLinePart.exists(lineData))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("The same line KML file ({0}) can not be use for a line element and for a polygon element", lineData.Path));
                return null;
            }

            LineLinePart part;
            if (!_partDictionary.TryGetValue(lineData, out part))
            {
                part = new LineLinePart(lineData);
                _partDictionary.Add(lineData, part);
            }

            return part;
        }

        private readonly KmlFileData _lineData;

        private LineLinePart(KmlFileData lineData)
        {
            _lineData = lineData;
        }

        internal GeoPoint Point1 { get { return _lineData.PointList[0]; } }
        internal GeoPoint Point2 { get { return _lineData.PointList[_lineData.PointList.Count - 1]; } }

        internal IAttachment Attachment1 { get; set; }
        internal IAttachment Attachment2 { get; set; }

        public List<GeoPoint> AttachmentLine
        {
            get
            {
                return _lineData.PointList;
            }
        }
    }
}
