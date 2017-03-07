#include "PolygonLook.h"
#include "ItemLook.h"

namespace map_server
{
    PolygonLook::PolygonLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, const std::string& textNameJson,
                             int contourZIndex, int contourAlpha, int contourRed, int contourGreen, int contourBlue, double contourSize, const std::string& contourNameJson,
                             int fillZIndex, int fillAlpha, int fillRed, int fillGreen, int fillBlue, const std::string& fillNameJson, IMap *iMap) :
        Look(id, new ItemLook(3 * id, 0, textAlpha, textRed, textGreen, textBlue, textSize, textNameJson, iMap)),
        _contourLook(new ItemLook(3 * id + 1, contourZIndex, contourAlpha, contourRed, contourGreen, contourBlue, contourSize, contourNameJson, iMap)),
        _fillLook(new ItemLook(3 * id + 2, fillZIndex, fillAlpha, fillRed, fillGreen, fillBlue, 0.0, fillNameJson, iMap))
    {
    }

    PolygonLook::~PolygonLook()
    {
		delete _contourLook;
		delete _fillLook;
    }
}
