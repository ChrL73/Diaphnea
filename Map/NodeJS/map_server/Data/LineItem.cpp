#include "LineItem.h"

namespace map_server
{
    const std::string& LineItem::getInfoJson(unsigned int resolutionIndex) const
    {
        if (resolutionIndex >= _infoJsonVector.size()) resolutionIndex = _infoJsonVector.size() - 1;

        if (_infoJsonVector[resolutionIndex].empty())
        {
            std::stringstream s;
            s << "{\"type\":\"line\",\"points\":[";
            addPointArray(s, resolutionIndex);
            s << "]}";
            _infoJsonVector[resolutionIndex] = s.str();
        }

        return _infoJsonVector[resolutionIndex];
    }
}
