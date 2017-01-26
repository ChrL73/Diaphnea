#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"

#include <cmath>

namespace map_server
{
    RepulsiveCenter::RepulsiveCenter(const TextDisplayerParameters *parameters, double x, double y, double axisDx, double axisDy, double radius1, double radius2, double u0, bool excluding, bool rectangle) :
        _x(x), _y(y), _u0(u0), _excluding(excluding), _rectangle(rectangle)
    {
        double r = sqrt(parameters->getPotentialTableSizeD() / (axisDx * axisDx + axisDy * axisDy)) / parameters->getMaxRepulsionRatio();
        _v11 = axisDx * r / radius1;
        _v12 = axisDy * r / radius1;
        _v21 = -axisDy * r / radius2;
        _v22 = axisDx * r / radius2;
    }
}
