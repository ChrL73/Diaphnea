/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class LineMapElement : MapElement
    {
        private readonly List<KmlFileData> _kmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedLineLineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedPolygonLineKmlFileList = new List<KmlFileData>();
        private readonly List<KmlFileData> _attachedPolygonPolygonKmlFileList = new List<KmlFileData>();

        private readonly List<LineLinePart> _partList = new List<LineLinePart>();

        internal List<LineLinePart> PartList { get { return _partList; } }

        internal LineMapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId) :
            base(id, mapData, name, shortName, importance, lookIds, categoryId) { }

        protected override bool allowMultiline() { return false; }

        internal List<GeoPoint> getPoints(XmlResolution resolution)
        {
            List<GeoPoint> points = new List<GeoPoint>();
            foreach (LineLinePart part in _partList) points.AddRange(part.getLine(resolution));
            return points;
        }

        internal double getDistance(GeoPoint point, XmlResolution resolution)
        {
            double d = Double.MaxValue;

            foreach (LineLinePart part in _partList)
            {
                double d1 = DistanceCalculator.getDistance(part.getLine(resolution), point);
                if (d1 < d) d = d1;
            }

            return d;
        }

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
            if (_kmlFileList.Count == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("No KML file associated to element '{0}'", Id));
                return -1;
            }

            return 0;
        }

        internal override int formParts2()
        {
            double dMaxInKm = MapData.XmlMapData.parameters.maxConnectionDistanceInKm;

            foreach (KmlFileData linePath in _kmlFileList)
            {
                LineLinePart part = LineLinePart.getPart(linePath, Id);
                if (part == null) return -1;

                List<IAttachment> attachCandidates = new List<IAttachment>();

                foreach (KmlFileData path in _kmlFileList)
                {
                    if (path != linePath)
                    {
                        LineLinePart part2 = LineLinePart.getPart(path, Id);
                        if (part2 == null) return -1;
                        attachCandidates.Add(part2);
                    }
                }

                foreach (KmlFileData path in _attachedLineLineKmlFileList)
                {
                    if (path != linePath)
                    {
                        LineLinePart part2 = LineLinePart.getPart(path, Id);
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
                    PolygonPolygonPart part2 = PolygonPolygonPart.getPart(path, Id);
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

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            codeGenerator.addLineElement(this);

            return 0;
        }
    }
}
