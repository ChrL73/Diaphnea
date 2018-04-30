#include "PointElement.h"

namespace map_server
{
    const PointLook *PointElement::getLook(int i) const
    {
        if (i >= _lookCount) i = _lookCount - 1;
        return reinterpret_cast<const PointLook *>(pointLooks + *(intArrays + _looks + i));
    }
}
