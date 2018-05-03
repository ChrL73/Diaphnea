#pragma once

namespace map_server
{
    class TextPotential
    {
    private:
        double _max;
        double _sum;
        double _n;
        bool _infinite;

        static constexpr double _alpha = 0.0;

    public:
        TextPotential(void) : _max(0.0), _sum(0.0), _n(0.0), _infinite(false) {}
        TextPotential(bool infinite) : _max(0.0), _sum(0.0), _n(0.0), _infinite(infinite) {}

        void add(double x);
        double getMax(void) const;
        double compareTo(const TextPotential& p) const;
    };
}
