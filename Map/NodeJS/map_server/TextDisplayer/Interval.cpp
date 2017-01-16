#include "Interval.h"

#include <utility>

namespace map_server
{
    const Interval Interval::getIntersection(const Interval& s) const
    {
        if (_emptySet || s._emptySet) return std::move(Interval());

        if (s._a < _a)
        {
            if (s._b < _b) return std::move(Interval(_a, s._b));
            return std::move(Interval(_a, _b));
        }

        if (_b < s._b) return std::move(Interval(s._a, _b));
        return std::move(Interval(s._a, s._b));
    }
}
