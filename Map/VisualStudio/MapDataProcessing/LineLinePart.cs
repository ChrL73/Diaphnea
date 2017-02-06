using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;

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
        private readonly DatabaseMapItem _smoothedLineMapItem = new DatabaseMapItem(false);
        private bool _smoothed = false;
        internal BsonValue MapItemId { get { return _smoothedLineMapItem.Id; } }

        private LineLinePart(KmlFileData lineData)
        {
            _lineData = lineData;
        }

        internal GeoPoint Point1 { get { return _lineData.PointList[0]; } }
        internal GeoPoint Point2 { get { return _lineData.PointList[_lineData.PointList.Count - 1]; } }

        internal IAttachment Attachment1 { get; set; }
        internal IAttachment Attachment2 { get; set; }

        public List<GeoPoint> AttachmentLine { get { return _lineData.PointList; } }
        public DatabaseMapItem SmoothedAttachmentLine { get { return _smoothedLineMapItem; } }

        internal static int smoothAll(MapData mapData)
        {
            foreach (LineLinePart part in _partDictionary.Values)
            {
                if (part.smooth(mapData) != 0) return -1;
            }

            return 0;
        }

        private int smooth(MapData mapData)
        {
            if (_smoothed) return 0;
            if (Attachment1 is LineLinePart) ((LineLinePart)Attachment1).smooth(mapData);
            if (Attachment2 is LineLinePart) ((LineLinePart)Attachment2).smooth(mapData);

            foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
            {
                List<GeoPoint> line = new List<GeoPoint>(_lineData.PointList);
                if (Attachment1 != null) line[0] = DistanceCalculator.getNearestPoint(Attachment1.SmoothedAttachmentLine.getPointList(resolution), Point1);
                if (Attachment2 != null) line[line.Count - 1] = DistanceCalculator.getNearestPoint(Attachment2.SmoothedAttachmentLine.getPointList(resolution), Point2);

                List<GeoPoint> smoothedLine = Smoother.smoothLine(line, resolution, _lineData.Path, null);
                if (smoothedLine == null) return -1;
                _smoothedLineMapItem.addLine(resolution, smoothedLine);
                if (KmlWriter.write(smoothedLine, KmlFileTypeEnum.LINE, "Lines", Path.GetFileName(_lineData.Path), resolution) != 0) return -1;
            }

            _smoothed = true;
            return 0;
        }

        internal static int fillDatabase(IMongoDatabase database, MapData mapData)
        {
            foreach (LineLinePart part in _partDictionary.Values)
            {
                if (part._smoothedLineMapItem.fillDataBase(database, mapData, Path.GetFileNameWithoutExtension(part._lineData.Path)) != 0) return -1;
            }

            return 0;
        }
    }
}
