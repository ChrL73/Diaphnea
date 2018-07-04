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
    class OrientedPolygonLinePart
    {
        private readonly PolygonLinePart _part;
        private readonly OrientationEnum _orientation;

        internal OrientedPolygonLinePart(PolygonLinePart part, OrientationEnum orientation)
        {
            _part = part;
            _orientation = orientation;
        }

        internal OrientationEnum Orientation { get { return _orientation; } }
        internal KmlFileData Line { get { return _part.Line; } }
        internal KmlFileData Point1 { get { return _orientation == OrientationEnum.DIRECT ? _part.Point1 : _part.Point2; } }
        internal KmlFileData Point2 { get { return _orientation == OrientationEnum.DIRECT ? _part.Point2 : _part.Point1; } }
        internal int MapItemCppOffset { get { return _part.MapItemCppOffset; } }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            List<GeoPoint> list = new List<GeoPoint>(_part.getPointList(resolution));
            if (_orientation == OrientationEnum.INVERSE) list.Reverse();
            return list;
        }
    }
}
