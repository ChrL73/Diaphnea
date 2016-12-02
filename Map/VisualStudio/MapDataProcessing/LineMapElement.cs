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
        private readonly List<KmlFileData> _kmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedLineLineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedPolygonLineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedPolygonPolygonKmlFileList = new List<KmlFileData>();

        private readonly List<LineLinePart> _partList = new List<LineLinePart>();

        internal LineMapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, string lookId) :
            base(id, mapData, name, shortName, lookId) { }

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
                _kmlFileList.Add(data);
            }

            return 0;
        }

        internal int attachLineLineKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);

            if (data.Type == KmlFileTypeEnum.LINE)
            {
                _attachedLineLineKmlFileList.Add(data);
            }
            else
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not attach KML file '{0}' as line data to line element '{1}'", path, Id));
                return -1;
            }

            return 0;
        }

        internal int attachPolygonLineKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);

            if (data.Type == KmlFileTypeEnum.LINE)
            {
                _attachedPolygonLineKmlFileList.Add(data);
            }
            else if (data.Type == KmlFileTypeEnum.POLYGON)
            {
                _attachedPolygonPolygonKmlFileList.Add(data);
            }

            return 0;
        }

        internal override int formParts1()
        {
            return 0;
        }

        internal override int formParts2()
        {
            double dMaxInKm = MapData.XmlMapData.parameters.maxConnectionDistanceInKm;

            foreach (KmlFileData linePath in _kmlFileList)
            {
                LineLinePart part = LineLinePart.getPart(linePath);
                if (part == null) return -1;

                List<IAttachment> attachCandidates = new List<IAttachment>();

                foreach (KmlFileData path in _kmlFileList)
                {
                    if (path != linePath)
                    {
                        LineLinePart part2 = LineLinePart.getPart(path);
                        if (part2 == null) return -1;
                        attachCandidates.Add(part2);
                    }
                }

                foreach (KmlFileData path in _attachedLineLineKmlFileList)
                {
                    if (path != linePath)
                    {
                        LineLinePart part2 = LineLinePart.getPart(path);
                        if (part2 == null) return -1;
                        attachCandidates.Add(part2);
                    }
                }

                foreach (KmlFileData path in _attachedPolygonLineKmlFileList)
                {
                    PolygonLinePart part2 = PolygonLinePart.getPart(path);
                    if (part2 == null) return -1;
                    attachCandidates.Add(part2);
                }

                foreach (KmlFileData path in _attachedPolygonPolygonKmlFileList)
                {
                    PolygonPolygonPart part2 = PolygonPolygonPart.getPart(path);
                    if (part2 == null) return -1;
                    attachCandidates.Add(part2);
                }

                double d1 = dMaxInKm;
                double d2 = dMaxInKm;
                IAttachment attachment1 = null;
                IAttachment attachment2 = null;

                foreach (IAttachment attachment in attachCandidates)
                {
                    double d = DistanceCalculator.getDistance(attachment.AttachmentLine, part.Point1);
                    if (d < d1)
                    {
                        d1 = d;
                        attachment1 = attachment;
                    }

                    d = DistanceCalculator.getDistance(attachment.AttachmentLine, part.Point2);
                    if (d < d2)
                    {
                        d2 = d;
                        attachment2 = attachment;
                    }
                }

                part.Attachment1 = attachment1;
                part.Attachment2 = attachment2;
                _partList.Add(part);
            }

            return 0;
        }

        internal override int fillDatabase(IMongoDatabase database)
        {
            return 0;
        }
    }
}
