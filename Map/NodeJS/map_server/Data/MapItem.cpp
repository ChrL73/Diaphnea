#include "MapItem.h"

#include <limits>

namespace map_server
{
    MapItem::MapItem(int id, int resolutionCount) :
        _id(id), _currentLook(0), _currentTextLook(0),
        _xMin(std::numeric_limits<double>::max()),
        _xMax(std::numeric_limits<double>::lowest()),
        _yMin(std::numeric_limits<double>::max()),
        _yMax(std::numeric_limits<double>::lowest())
    {
        _infoJsonVector.resize(resolutionCount);
    }

    const std::string& MapItem::getInfoJson(unsigned int resolutionIndex) const
    {
        if (resolutionIndex < _infoJsonVector.size()) return _infoJsonVector[resolutionIndex];
        return _infoJsonVector[0];
    }
}
