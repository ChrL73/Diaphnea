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
        Interval(void) : _emptySet(true) {}

        const Interval getIntersection(const Interval& s) const;

    };
}
