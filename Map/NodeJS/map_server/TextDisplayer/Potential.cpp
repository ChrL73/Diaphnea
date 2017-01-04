#include "Potential.h"

namespace map_server
{
    Potential::Potential(double value)
    {
        if (value > 0.0)
        {
            _value = value;
            _sum = value;
            _max = value;
        }
        else
        {
            _value = 0.0;
            _sum = 0.0;
            _max = 0.0;
        }
    }

    void Potential::add(double value)
    {
        if (value > 0.0)
        {
            _sum += value;
            if (value > _max) _max = value;
            _value = (1.0 - _alpha) * _max + _alpha * _sum;
        }
    }

    Potential& Potential::operator+=(const Potential& p)
    {
        _sum += p._sum;
        if (p._max > _max) _max = p._max;
        _value = (1.0 - _alpha) * _max + _alpha * _sum;

        return *this;
    }

    /*const Potential Potential::operator+(const Potential& p) const
    {
        Potential result = *this;

        result._sum += p._sum;
        if (p._max > result._max) result._max = p._max;
        result._value = (1.0 - _alpha) * result._max + _alpha * result._sum;

        return result;
    }*/
}
