#pragma once

namespace map_server
{
    class Potential
    {
    private:
        double _exlcudingTerm;
        double _notExlcudingTerm;

    public:
        Potential(void) : _exlcudingTerm(0.0), _notExlcudingTerm(0.0) {}
        Potential(double exlcudingTerm, double notExlcudingTerm) :
            _exlcudingTerm(exlcudingTerm), _notExlcudingTerm(notExlcudingTerm) { }

        void addExcludingTerm(double p);
        void addNotExcludingTerm (double p);

        const Potential operator+(const Potential& p);
        double compareTo(const Potential& p);
        bool isAcceptable(const Potential& threshold);
    };
}
