 #pragma once

 namespace map_server
 {
    class Point
    {
    private:
        const double _x;
        const double _y;

    public:
        Point(double x, double y) : _x(x), _y(y) {}

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
    };
 }
