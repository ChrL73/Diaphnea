#include "RepulsiveCenter.h"

#include <cmath>

namespace map_server
{
    double RepulsiveCenter::_maxRepulsionRatio = 5.0;
    int RepulsiveCenter::_potentialTableSize = 10000;

    RepulsiveCenter::RepulsiveCenter(double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding) :
        _x(x), _y(y), _u0(u0), _excluding(excluding)
    {
        double r = sqrt(_potentialTableSize / (axisDx * axisDx + axisDy * axisDy)) / _maxRepulsionRatio;
        _v11 = axisDx * r / radius1;
        _v12 = axisDy * r / radius1;
        _v21 = -axisDy * r / radius2;
        _v22 = axisDx * r / radius2;

        axisDy *= 2.0;
        r = 1.0 / sqrt(axisDx * axisDx + axisDy * axisDy);
        _cosAlpha = -r * axisDy;
        _sinAlpha = r * axisDx;
    }
}
