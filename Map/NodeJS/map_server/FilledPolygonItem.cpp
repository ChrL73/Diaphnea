#include "FilledPolygonItem.h"

namespace map_server
{
    void FilledPolygonItem::setInfoJsonVector(void)
    {
        int i;
        for (i = 0; i < _resolutionCount; ++i)
        {
            std::stringstream s;
            s << "{\"type\":\"polygon\",\"points\":[";
            addPointArray(s, i);
            s << "]}";
            _infoJsonVector[i] = s.str();
        }
    }
}
