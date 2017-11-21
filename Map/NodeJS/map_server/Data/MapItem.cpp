#include "MapItem.h"

#include <limits>

namespace map_server
{
    MapItem::MapItem(int id, int resolutionCount) :
        _id(id), _currentCategory(0), _nameMapPtr(0), _importance(0.0),
        _xMin(std::numeric_limits<double>::max()),
        _xMax(std::numeric_limits<double>::lowest()),
        _yMin(std::numeric_limits<double>::max()),
        _yMax(std::numeric_limits<double>::lowest())
    {
        _infoJsonVector.resize(resolutionCount);
    }

    const ItemLook *MapItem::getCurrentLook(int i) const
    {
        int n = _currentLooks.size() - 1;
        if (n < 0) return 0;
        if (i <= n) return _currentLooks[i];
        return _currentLooks[n];
    }

    const ItemLook *MapItem::getCurrentTextLook(int i) const
    {
        int n = _currentTextLooks.size() - 1;
        if (n < 0) return 0;
        if (i <= n) return _currentTextLooks[i];
        return _currentTextLooks[n];
    }

    const std::string& MapItem::getInfoJson(unsigned int resolutionIndex) const
    {
        if (resolutionIndex < _infoJsonVector.size()) return _infoJsonVector[resolutionIndex];
        return _infoJsonVector[_infoJsonVector.size() - 1];
    }

    int MapItem::getTextCount(const std::string& languageId) const
    {
        if (_nameMapPtr == 0) return 0;
        std::map<std::string, std::vector<ElementName *> >::iterator it = _nameMapPtr->find(languageId);
        if (it == _nameMapPtr->end()) return 0;
        return (*it).second.size();
    }

    const ElementName *MapItem::getText(const std::string& languageId, int i) const
    {
        if (_nameMapPtr == 0) return 0;
        std::map<std::string, std::vector<ElementName *> >::iterator it = _nameMapPtr->find(languageId);
        if (it == _nameMapPtr->end()) return 0;
        return (*it).second[i];
    }
}
