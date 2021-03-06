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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class PolygonLinePart : IAttachment
    {
        private static Dictionary<KmlFileData, PolygonLinePart> _partDictionary = new Dictionary<KmlFileData, PolygonLinePart>();

        internal static void clearAll()
        {
            _partDictionary.Clear();
        }

        internal static bool exists(KmlFileData lineData)
        {
            return _partDictionary.ContainsKey(lineData);
        }

        internal static PolygonLinePart getPart(KmlFileData lineData, KmlFileData pointData1, KmlFileData pointData2, string elementId)
        {
            if (LineLinePart.exists(lineData))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("The same line KML file ({0}) can not be use for a line element and for a polygon element", lineData.Path));
                return null;
            }

            PolygonLinePart part;
            if (!_partDictionary.TryGetValue(lineData, out part))
            {
                part = new PolygonLinePart(lineData, pointData1, pointData2/*, elementId*/);
                _partDictionary.Add(lineData, part);
            }
            else
            {
                if (pointData1 != part._pointData1)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Different point1s ('{0}' and '{1}') for line polygon part '{2}'",
                                             pointData1.Path, part._pointData1.Path, lineData.Path));
                    return null;
                }
                else if (pointData2 != part._pointData2)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Different point2s ('{0}' and '{1}') for line polygon part '{2}'",
                                             pointData2.Path, part._pointData2.Path, lineData.Path));
                    return null;
                }
            }

            return part;
        }

        internal static PolygonLinePart getPart(KmlFileData lineData)
        {
            PolygonLinePart part;
            if (_partDictionary.TryGetValue(lineData, out part)) return part;

            MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Polygon line part '{0}' does not exists", lineData.Path, part._pointData2.Path, lineData.Path));
            return null;
        }

        private readonly KmlFileData _lineData;
        private readonly KmlFileData _pointData1;
        private readonly KmlFileData _pointData2;
        private readonly DatabaseMapItem _smoothedLineMapItem;

        private PolygonLinePart(KmlFileData lineData, KmlFileData pointData1, KmlFileData pointData2/*, string element0Id*/)
        {
            _lineData = lineData;
            _pointData1 = pointData1;
            _pointData2 = pointData2;
            _smoothedLineMapItem = new DatabaseMapItem(false/*, element0Id*/);
        }

        internal KmlFileData Line { get { return _lineData; } }
        internal KmlFileData Point1 { get { return _pointData1; } }
        internal KmlFileData Point2 { get { return _pointData2; } }
        internal int MapItemCppOffset { get { return _smoothedLineMapItem.CppOffset; } }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            return _smoothedLineMapItem.getPointList(resolution);
        }

        internal static int smoothAll(MapData mapData)
        {
            foreach (PolygonLinePart part in _partDictionary.Values)
            {
                List<GeoPoint> line = new List<GeoPoint>();
                line.Add(part.Point1.PointList[0]);
                line.AddRange(part.Line.PointList);
                line.Add(part.Point2.PointList[0]);

                foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
                {
                    List<GeoPoint> smoothedLine = Smoother.smoothLine(line, resolution, part.Line.Path, mapData.LineSuperposerDictionary[resolution], true);
                    if (smoothedLine == null) return -1;
                    part._smoothedLineMapItem.addLine(resolution, smoothedLine);
                    if (KmlWriter.write(smoothedLine, KmlFileTypeEnum.LINE, "Polygons_Lines", Path.GetFileName(part.Line.Path), resolution) != 0) return -1;
                }
            }

            return 0;
        }

        internal static int generateCode(CodeGenerator codeGenerator, MapData mapData)
        {
            foreach (PolygonLinePart part in _partDictionary.Values)
            {
                if (part._smoothedLineMapItem.generateCode(codeGenerator, mapData, Path.GetFileNameWithoutExtension(part.Line.Path)) != 0) return -1;
            }

            return 0;
        }

        public List<GeoPoint> AttachmentLine
        {
            get
            {
                List<GeoPoint> line = new List<GeoPoint>();
                line.Add(_pointData1.PointList[0]);
                line.AddRange(_lineData.PointList);
                line.Add(_pointData2.PointList[0]); 
                return line;
            }
        }

        public DatabaseMapItem SmoothedAttachmentLine { get { return _smoothedLineMapItem; } }
    }
}
