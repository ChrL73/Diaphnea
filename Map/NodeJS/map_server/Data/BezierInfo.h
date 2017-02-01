 #pragma once

 namespace map_server
 {
    class BezierInfo
    {
    private:
        const double _x1;
        const double _y1;
        const double _x2;
        const double _y2;

    public:
        BezierInfo(double x1, double y1, double x2, double y2) : _x1(x1), _y1(y1), _x2(x2), _y2(y2) {}

        double getX1(void) const { return _x1; }
        double getY1(void) const { return _y1; }
        double getX2(void) const { return _x2; }
        double getY2(void) const { return _y2; }

    };
 }
