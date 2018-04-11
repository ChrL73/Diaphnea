﻿using MongoDB.Bson;
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
        internal BsonValue MapItemId { get { return _part.MapItemId; } }

        internal List<GeoPoint> getPointList(XmlResolution resolution)
        {
            List<GeoPoint> list = new List<GeoPoint>(_part.getPointList(resolution));
            if (_orientation == OrientationEnum.INVERSE) list.Reverse();
            return list;
        }
    }
}
