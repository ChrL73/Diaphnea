 #pragma once

 namespace map_server
 {
    class BezierInfo;

    class Point
    {
    private:
        const BezierInfo * const _bezierInfo;
        const double _x;
        const double _y;

    public:
        Point(double x1, double y1, double x2, double y2, double x, double y);
        Point(double x, double y) : _bezierInfo(0), _x(x), _y(y) {}
        ~Point();

        const BezierInfo *getBezierInfo(void) const { return _bezierInfo; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
 }
