#include "PolygonElement.h"

namespace map_server
{
    const PolygonLook *PolygonElement::getLook(int i) const
    {
        if (i >= _lookCount) i = _lookCount - 1;
        return reinterpret_cast<const PolygonLook *>(polygonLooks + *(intArrays + _looks + i));
    }
}
