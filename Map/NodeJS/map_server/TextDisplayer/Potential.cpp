#include "Potential.h"

namespace map_server
{
    void Potential::addExcludingTerm(double p)
    {
        if (p > _excludingTerm) _excludingTerm = p;
        //_excludingTerm += p;
    }

    void Potential::addNotExcludingTerm(double p)
    {
        if (p > _notExcludingTerm) _notExcludingTerm = p;
        //_notExcludingTerm += p;
    }

    Potential& Potential::operator+=(const Potential& p)
    {
        if (p._excludingTerm > _excludingTerm) _excludingTerm = p._excludingTerm;
        if (p._notExcludingTerm > _notExcludingTerm) _notExcludingTerm = p._notExcludingTerm;
        /*_excludingTerm += p._excludingTerm;
        _notExcludingTerm += p._notExcludingTerm;*/
        return *this;
    }

    const Potential Potential::operator+(const Potential& p) const
    {
        Potential result = *this;
        if (p._excludingTerm > result._excludingTerm) result._excludingTerm = p._excludingTerm;
        if (p._notExcludingTerm > result._notExcludingTerm) result._notExcludingTerm = p._notExcludingTerm;
        /*result._excludingTerm += p._excludingTerm;
        result._notExcludingTerm += p._notExcludingTerm;*/
        return result;
    }

    double Potential::compareTo(const Potential& p) const
    {
        double p1 = _excludingTerm > _notExcludingTerm ? _excludingTerm : _notExcludingTerm;
        double p2 = p._excludingTerm > p._notExcludingTerm ? p._excludingTerm : p._notExcludingTerm;
        //return _excludingTerm + _notExcludingTerm - p._excludingTerm - p._notExcludingTerm;
        return p1 - p2;
    }

    bool Potential::isAcceptable(const Potential& threshold) const
    {
        return _excludingTerm < threshold._excludingTerm && _notExcludingTerm < threshold._notExcludingTerm;
    }
}
