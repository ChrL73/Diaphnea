#pragma once

#include <vector>

namespace map_server
{
    class Interval
    {
    private:
        double _a;
        double _b;
        bool _emptySet;

    public:
        Interval(double a, double b) : _a(a), _b(b), _emptySet(a > b) {}
        Interval(void) : _a(1.0), _b(0.0), _emptySet(true) {}

        double getA(void) const { return _a; }
        double getB(void) const { return _b; }
        bool isEmpty(void) const { return _emptySet; }

        const Interval getIntersection(const Interval& i) const;

    };
}
