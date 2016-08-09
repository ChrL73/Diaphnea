#pragma once

namespace produce_questions
{
    class Point
    {
    private:
        const double _x;
        const double _y;
        const double _z;

    public:
        Point(double x, double y, double z) : _x(x), _y(y), _z(z) {}

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getZ(void) const { return _z; }
    };
}
