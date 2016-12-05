#include "ItemLook.h"
#include "IMap.h"

#include <cstdio>

namespace map_server
{
    ItemLook::ItemLook(int id, int zIndex, int alpha, int red, int green, int blue, double size, IMap *iMap) :
        _id(id), _zIndex(zIndex)/*, _alpha(alpha), _red(red), _green(green), _blue(blue), _size(size)*/
    {
#if _WIN32
        sprintf_s(_json, 128, "{\"zI\":%d,\"a\":%d,\"r\":%d,\"g\":%d,\"b\":%d,\"size\":%.5g}", zIndex, alpha, red, green, blue, size);
#else
        sprintf(_json, "{\"zI\":%d,\"a\":%d,\"r\":%d,\"g\":%d,\"b\":%d,\"size\":%.5g}", zIndex, alpha, red, green, blue, size);
#endif
        iMap->addItemLook(this);
    }
}
