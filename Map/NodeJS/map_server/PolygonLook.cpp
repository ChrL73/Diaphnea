#include "PolygonLook.h"
#include "ItemLook.h"

namespace map_server
{
    PolygonLook::PolygonLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize,
                    int contourZIndex, int contourAlpha, int contourRed, int contourGreen, int contourBlue, double contourSize, double fillLightRatio, int fillZIndex, IMap *iMap) :
        Look(id, new ItemLook(3 * id, 0, textAlpha, textRed, textGreen, textBlue, textSize, iMap)),
        _contourLook(new ItemLook(3 * id + 1, contourZIndex, contourAlpha, contourRed, contourGreen, contourBlue, contourSize, iMap)),
        _fillLook(new ItemLook(3 * id + 2, fillZIndex, contourAlpha,
                               255 - static_cast<int>(static_cast<double>(255 - contourRed) * fillLightRatio),
                               255 - static_cast<int>(static_cast<double>(255 - contourGreen) * fillLightRatio),
                               255 - static_cast<int>(static_cast<double>(255 - contourBlue) * fillLightRatio), 0.0, iMap))
    {
    }

    PolygonLook::~PolygonLook()
    {
    }
}
