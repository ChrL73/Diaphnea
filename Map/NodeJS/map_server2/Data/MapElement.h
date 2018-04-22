#pragma once

namespace map_server
{
    class PointElement;
    class LineElement;
    class PolygonElement;

    class MapElement
    {
    private:
        const PointElement * const _pointElement;
        const LineElement * const _lineElement;
        const PolygonElement * const _polygonElement;

    public:
        MapElement(const PointElement *pointElement) :
            _pointElement(pointElement), _lineElement(0), _polygonElement(0) {}

        MapElement(const LineElement *lineElement) :
            _pointElement(0), _lineElement(lineElement), _polygonElement(0) {}

        MapElement(const PolygonElement *polygonElement) :
            _pointElement(0), _lineElement(0), _polygonElement(polygonElement) {}
    };
}

