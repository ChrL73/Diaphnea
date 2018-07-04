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
using System.IO;
using Schemas;

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

        internal static LineLinePart getPart(KmlFileData lineData, string elementId)
        {
            if (PolygonLinePart.exists(lineData))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("The same line KML file ({0}) can not be use for a line element and for a polygon element", lineData.Path));
                return null;
            }

            LineLinePart part;
            if (!_partDictionary.TryGetValue(lineData, out part))
            {
                part = new LineLinePart(lineData/*, elementId*/);
                _partDictionary.Add(lineData, part);
            }

            return part;
        }

        private readonly KmlFileData _lineData;
        private readonly bool _superposable;
        private readonly DatabaseMapItem _smoothedLineMapItem;
        private bool _smoothed = false;
        internal int MapItemCppOffset { get { return _smoothedLineMapItem.CppOffset; } }

        private LineLinePart(KmlFileData lineData/*, string element0Id*/)
        {
            _lineData = lineData;
            _superposable = (!Path.GetFileName(_lineData.Path).Contains("!"));
            _smoothedLineMapItem = new DatabaseMapItem(false/*, element0Id*/);
        }

        internal List<GeoPoint> getLine(XmlResolution resolution)
        {
            return _smoothedLineMapItem.getLine(resolution);
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
            if (Attachment1 is LineLinePart)
            {
                _smoothedLineMapItem.Cap1Round = false;
                ((LineLinePart)Attachment1).smooth(mapData);
            }

            if (Attachment2 is LineLinePart)
            {
                _smoothedLineMapItem.Cap2Round = false;
                ((LineLinePart)Attachment2).smooth(mapData);
            }

            foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
            {
                List<GeoPoint> line = new List<GeoPoint>(_lineData.PointList);
                if (Attachment1 != null) line[0] = DistanceCalculator.getNearestPoint(Attachment1.SmoothedAttachmentLine.getPointList(resolution), Point1);
                if (Attachment2 != null) line[line.Count - 1] = DistanceCalculator.getNearestPoint(Attachment2.SmoothedAttachmentLine.getPointList(resolution), Point2);

                LineSuperposer superposer = _superposable ? mapData.LineSuperposerDictionary[resolution] : null;
                List <GeoPoint> smoothedLine = Smoother.smoothLine(line, resolution, _lineData.Path, superposer, false);
                if (smoothedLine == null) return -1;
                _smoothedLineMapItem.addLine(resolution, smoothedLine);
                if (KmlWriter.write(smoothedLine, KmlFileTypeEnum.LINE, "Lines", Path.GetFileName(_lineData.Path), resolution) != 0) return -1;
            }

            _smoothed = true;
            return 0;
        }

        internal static int generateCode(CodeGenerator codeGenerator, MapData mapData)
        {
            foreach (LineLinePart part in _partDictionary.Values)
            {
                if (part._smoothedLineMapItem.generateCode(codeGenerator, mapData, Path.GetFileNameWithoutExtension(part._lineData.Path)) != 0) return -1;
            }

            return 0;
        }
    }
}
