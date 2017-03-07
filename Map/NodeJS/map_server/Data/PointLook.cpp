#include "PointLook.h"
#include "ItemLook.h"

namespace map_server
{
    PointLook::PointLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, const std::string& textNameJson,
                         int pointZIndex, int pointAlpha, int pointRed, int pointGreen, int pointBlue, double pointSize, const std::string& pointNameJson, IMap *iMap) :
        Look(id, new ItemLook(3 * id , 0, textAlpha, textRed, textGreen, textBlue, textSize, textNameJson, iMap)),
        _pointLook(new ItemLook(3 * id + 1, pointZIndex, pointAlpha, pointRed, pointGreen, pointBlue, pointSize, pointNameJson, iMap))
    {
    }

    PointLook::~PointLook()
    {
        delete _pointLook;
    }
}
