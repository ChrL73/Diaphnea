#pragma once

namespace map_server
{
    class TextDisplayerParameters;

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
        RepulsiveCenter(const TextDisplayerParameters *parameters, double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding);

        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getU0(void) const { return _u0; }
        bool getExcluding(void) const { return _excluding; }
        double getV11(void) const { return _v11; }
        double getV12(void) const { return _v12; }
        double getV21(void) const { return _v21; }
        double getV22(void) const { return _v22; }
    };
}
