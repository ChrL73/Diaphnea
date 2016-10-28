#include "PointLook.h"
#include "ItemLook.h"

namespace map_server
{
    PointLook::PointLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                  int pointZIndex, int pointAlpha, int pointRed, int pointGreen, int pointBlue, double pointSize, IMap *iMap) :
        Look(id, new ItemLook(3 * id , 0, textAlpha, textRed, textGreen, textBlue, textSize, iMap)),
        _pointLook(new ItemLook(3 * id + 1, pointZIndex, pointAlpha, pointRed, pointGreen, pointBlue, pointSize, iMap))
    {
    }

    PointLook::~PointLook()
    {
        delete _pointLook;
    }
}
