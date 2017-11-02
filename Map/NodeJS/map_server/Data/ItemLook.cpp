#include "ItemLook.h"
#include "IMap.h"

#include <cstdio>
#include <sstream>

namespace map_server
{
    ItemLook::ItemLook(int id, int zIndex, int alpha, int red, int green, int blue, double size, IMap *iMap) :
        _id(id), _zIndex(zIndex), _alpha(alpha), _red(red), _green(green), _blue(blue), _size(size)
    {
        std::stringstream json;
        json << "{\"zI\":" << zIndex << ",\"a\":" << alpha << ",\"r\":" << red << ",\"g\":" << green << ",\"b\":" << blue << ",\"size\":"
             << size << "}";
        _json = json.str();

#if _WIN32
        sprintf_s(_hexColor, 8, "#%02X%02X%02X", red, green, blue);
#else
        sprintf(_hexColor, "#%02X%02X%02X", red, green, blue);
#endif
        iMap->addItemLook(this);
    }
}
