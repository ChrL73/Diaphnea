#include "LineLook.h"
#include "ItemLook.h"

namespace map_server
{
    LineLook::LineLook(int id, int textAlpha, int textRed, int textGreen, int textBlue, double textSize, const std::string& textNameJson,
                       int lineZIndex, int lineAlpha, int lineRed, int lineGreen, int lineBlue, double lineSize, const std::string& lineNameJson, IMap *iMap) :
        Look(id, new ItemLook(3 * id, 0, textAlpha, textRed, textGreen, textBlue, textSize, textNameJson, iMap)),
        _lineLook(new ItemLook(3 * id + 1, lineZIndex, lineAlpha, lineRed, lineGreen, lineBlue, lineSize, lineNameJson, iMap))
    {
    }

    LineLook::~LineLook()
    {
        delete _lineLook;
    }
}
