#include "LineElement.h"

namespace map_server
{
    const LineLook *LineElement::getLook(int i) const
    {
        if (i >= _lookCount) i = _lookCount - 1;
        return reinterpret_cast<const LineLook *>(lineLooks + *(intArrays + _looks + i));
    }
}
