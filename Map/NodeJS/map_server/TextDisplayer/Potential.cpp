#include "Potential.h"

#include <limits>

namespace map_server
{
    Potential Potential::MaxValue(std::numeric_limits<double>::max(), std::numeric_limits<double>::max());
    Potential Potential::MinValue(-std::numeric_limits<double>::max(), -std::numeric_limits<double>::max());

    void Potential::addExculdingTerm(double p)
    {
        _exlcudingTerm += p;
    }

    void Potential::addNotExculdingTerm (double p)
    {
        _notExlcudingTerm += p;
    }

    const Potential Potential::operator+(const Potential& p)
    {
        Potential result = *this;
        result._exlcudingTerm += p._exlcudingTerm;
        result._notExlcudingTerm += p._notExlcudingTerm;
        return result;
    }

    double Potential::compareTo(const Potential& p)
    {
        return _exlcudingTerm + _notExlcudingTerm - p._exlcudingTerm - p._notExlcudingTerm;
    }

    bool Potential::isAcceptable(const Potential& threshold)
    {
        return _exlcudingTerm < threshold._exlcudingTerm && _notExlcudingTerm < threshold._notExlcudingTerm;
    }
}
