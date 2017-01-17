#include "Interval.h"

#include <utility>

namespace map_server
{
    const Interval Interval::getIntersection(const Interval& i) const
    {
        if (_emptySet || i._emptySet) return std::move(Interval());

        if (i._a < _a)
        {
            if (i._b < _b) return std::move(Interval(_a, i._b));
            return std::move(Interval(_a, _b));
        }

        if (_b < i._b) return std::move(Interval(i._a, _b));
        return std::move(Interval(i._a, i._b));
    }
}
