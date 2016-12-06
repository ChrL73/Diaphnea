#include "LineItem.h"

namespace map_server
{
    void LineItem::setInfoJsonVector(void)
    {
        int i;
        for (i = 0; i < _resolutionCount; ++i)
        {
            std::stringstream s;
            s << "{\"type\":\"line\",\"points\":[";
            addPointArray(s, i);
            s << "]}";
            _infoJsonVector[i] = s.str();
        }
    }
}
