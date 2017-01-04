#pragma once

namespace map_server
{
    class Potential
    {
    private:
        double _value;
        double _sum;
        double _max;

        static constexpr double _alpha = 0.2;

        // If _alpha = 0.0: Potential(v1) + Potential(v2) + .. + Potential(vn) = Potential(max(v1, v2, ..., vn))
        // If _alpha = 1.0: Potential(v1) + Potential(v2) + .. + Potential(vn) = Potential(v1 + v2 + ... + vn)
        // If 0.0 < _alpha < 1.0: Sum = Linear interpolation between the 2 above terms

    public:
        Potential(void) : _value(0.0), _sum(0.0), _max(0.0) {}
        Potential(double value);

        void add(double value);
        Potential& operator+=(const Potential& p);
        //const Potential operator+(const Potential& p) const;
        double getValue(void) const { return _value; }
    };
}
