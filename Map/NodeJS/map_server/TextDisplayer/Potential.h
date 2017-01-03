#pragma once

namespace map_server
{
    class Potential
    {
    private:
        double _excludingTerm;
        double _notExcludingTerm;

    public:
        Potential(void) : _excludingTerm(0.0), _notExcludingTerm(0.0) {}
        Potential(double excludingTerm, double notExcludingTerm) :
            _excludingTerm(excludingTerm), _notExcludingTerm(notExcludingTerm) { }

        void addExcludingTerm(double p);
        void addNotExcludingTerm (double p);

        Potential& operator+=(const Potential& p);
        const Potential operator+(const Potential& p) const;
        double compareTo(const Potential& p) const;
        bool isAcceptable(const Potential& threshold) const;

        double getExcludingTerm(void) const { return _excludingTerm; }
        double getNotExcludingTerm(void) const { return _notExcludingTerm; }
    };
}
