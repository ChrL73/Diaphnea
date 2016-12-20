#pragma once

namespace map_server
{
    class IDisplayerConstants;

    class RepulsiveCenter
    {
    private:
        const double _x;
        const double _y;
        const double _u0;
        const bool _excluding;
        double _v11;
        double _v12;
        double _v21;
        double _v22;
        double _cosAlpha;
        double _sinAlpha;

    public:
        RepulsiveCenter(const IDisplayerConstants *constants, double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding);

    };
}
