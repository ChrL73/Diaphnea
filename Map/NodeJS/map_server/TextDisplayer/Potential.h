#pragma once

namespace map_server
{
    class Potential
    {
    private:
        double _exlcudingTerm;
        double _notExlcudingTerm;

    public:
        Potential(double exlcudingTerm, double notExlcudingTerm) :
            _exlcudingTerm(exlcudingTerm), _notExlcudingTerm(notExlcudingTerm) { }

        void addExculdingTerm(double p);
        void addNotExculdingTerm (double p);

        const Potential operator+(const Potential& p);
        double compareTo(const Potential& p);
        bool isAcceptable(const Potential& threshold);
    };
}