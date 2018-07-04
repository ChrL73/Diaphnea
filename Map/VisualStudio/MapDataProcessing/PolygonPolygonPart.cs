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
    class PolygonPolygonPart : IAttachment
    {
        private static Dictionary<KmlFileData, PolygonPolygonPart> _partDictionary = new Dictionary<KmlFileData, PolygonPolygonPart>();

        internal static void clearAll()
        {
            _partDictionary.Clear();
        }

        internal static PolygonPolygonPart getPart(KmlFileData polygonData, string elementId)
        {
            PolygonPolygonPart part;
            if (!_partDictionary.TryGetValue(polygonData, out part))
            {
                part = new PolygonPolygonPart(polygonData/*, elementId*/);
                _partDictionary.Add(polygonData, part);
            }

            return part;
        }

        private readonly KmlFileData _polygonData;
        private readonly DatabaseMapItem _smoothedPolygonMapItem;
        internal int MapItemCppOffset { get { return _smoothedPolygonMapItem.CppOffset; } }

        private PolygonPolygonPart(KmlFileData polygonData/*, string element0Id*/)
        {
            _polygonData = polygonData;
            _smoothedPolygonMapItem = new DatabaseMapItem(false/*, element0Id*/);
        }

        internal KmlFileData Polygon { get { return _polygonData; } }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            return _smoothedPolygonMapItem.getPointList(resolution);
        }

        internal static int smoothAll(MapData mapData)
        {
            foreach (PolygonPolygonPart part in _partDictionary.Values)
            {
                List<GeoPoint> line = new List<GeoPoint>(part.Polygon.PointList);

                foreach (XmlResolution resolution in mapData.XmlMapData.resolutionList)
                {
                    List<GeoPoint> smoothedPolygon = Smoother.smoothPolygon(line, resolution, part.Polygon.Path);
                    if (smoothedPolygon == null) return -1;
                    part._smoothedPolygonMapItem.addLine(resolution, smoothedPolygon);
                    KmlWriter.write(smoothedPolygon, KmlFileTypeEnum.POLYGON, "Polygons_Polygons", Path.GetFileName(part.Polygon.Path), resolution);
                }
            }

            return 0;
        }

        internal static int generateCode(CodeGenerator codeGenerator, MapData mapData)
        {
            foreach (PolygonPolygonPart part in _partDictionary.Values)
            {
                if (part._smoothedPolygonMapItem.generateCode(codeGenerator, mapData, Path.GetFileNameWithoutExtension(part.Polygon.Path)) != 0) return -1;
            }

            return 0;
        }

        public List<GeoPoint> AttachmentLine { get { return _polygonData.PointList; } }
        public DatabaseMapItem SmoothedAttachmentLine { get { return _smoothedPolygonMapItem; } }
    }
}
