#include "TextPotential.h"

#include <limits>

namespace map_server
{
    void TextPotential::add(double x)
    {
        if (x >= 0.0)
        {
            if (x > _max) _max = x;
            _sum += x;
            _n += 1.0;
        }
    }

    double TextPotential::getMax(void) const
    {
        if (_infinite) return std::numeric_limits<double>::max();
        return _max;
    }

    double TextPotential::compareTo(const TextPotential& p) const
    {
        if (p._infinite && !_infinite) return -1.0;
        if (_infinite && !p._infinite) return 1.0;

        double average1 = 0.0;
        if (_n != 0.0) average1 = _sum / _n;

        double average2 = 0.0;
        if (p._n != 0.0) average2 = p._sum / p._n;

        return _alpha * (average1 - average2) + (1.0 - _alpha) * (_max - p._max);
    }
}
