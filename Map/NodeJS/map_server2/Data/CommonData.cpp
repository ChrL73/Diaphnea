#include "CommonData.h"
#include "ElementInfo.h"
#include "MapData.h"
#include "PointElement.h"
#include "LineElement.h"
#include "PolygonElement.h"

namespace map_server
{
    CommonData::CommonData(void) : _lastElementOffset(0)
    {
        int i;
        for (i = 0; i < pointElementCount; ++i)
        {
            int offset = 13 * i;
            const char *id = reinterpret_cast<const char *>(strings) + pointElements[offset];
            ElementInfo *info = new ElementInfo(map_server::POINT, offset);
            _elementInfoMap.insert(std::pair<std::string, ElementInfo *>(id, info));
        }

        for (i = 0; i < lineElementCount; ++i)
        {
            int offset = 10 * i;
            const char *id = reinterpret_cast<const char *>(strings) + lineElements[offset];
            ElementInfo *info = new ElementInfo(map_server::LINE, offset);
            _elementInfoMap.insert(std::pair<std::string, ElementInfo *>(id, info));
        }

        for (i = 0; i < polygonElementCount; ++i)
        {
            int offset = 13 * i;
            const char *id = reinterpret_cast<const char *>(strings) + polygonElements[offset];
            ElementInfo *info = new ElementInfo(map_server::POLYGON, offset);
            _elementInfoMap.insert(std::pair<std::string, ElementInfo *>(id, info));
        }

        for (i = 0; i < languageCount; ++i)
        {
            const char *id = reinterpret_cast<const char *>(strings) + languageIds[i];
            _languageIdMap.insert(std::pair<std::string, int>(id, i));
        }
    }

    CommonData::~CommonData()
    {
        std::map<std::string, ElementInfo *>::iterator it = _elementInfoMap.begin();
        for (; it != _elementInfoMap.end(); ++it) delete (*it).second;
    }

    ElementTypeEnum CommonData::getElementType(const std::string& elementId)
    {
        std::map<std::string, ElementInfo *>::const_iterator it = _elementInfoMap.find(elementId);

        if (it != _elementInfoMap.end())
        {
            _lastElementOffset = (*it).second->getOffset();
            return (*it).second->getType();
        }

        return map_server::NO_TYPE;
    }

    const PointElement *CommonData::getLastElementAsPoint(void) const
    {
        return PointElement::get(_lastElementOffset);
    }

    const LineElement *CommonData::getLastElementAsLine(void) const
    {
        return LineElement::get(_lastElementOffset);
    }

    const PolygonElement *CommonData::getLastElementAsPolygon(void) const
    {
        return PolygonElement::get(_lastElementOffset);
    }

    const char *CommonData::getElementInfoJson(const std::string& elementId)
    {
        std::map<std::string, ElementInfo *>::const_iterator it = _elementInfoMap.find(elementId);

        if (it != _elementInfoMap.end())
        {
            int elementOffset = (*it).second->getOffset();
            ElementTypeEnum elementType = (*it).second->getType();

            if (elementType == map_server::POINT) return PointElement::get(elementOffset)->getInfoJson();
            else if (elementType == map_server::LINE) return LineElement::get(elementOffset)->getInfoJson();
            else if (elementType == map_server::POLYGON) return PolygonElement::get(elementOffset)->getInfoJson();
        }

        return 0;
    }

    int CommonData::getLanguageIndex(const std::string& languageId) const
    {
        std::map<std::string, int>::const_iterator it = _languageIdMap.find(languageId);
        if (it != _languageIdMap.end()) return (*it).second;
        return -1;
    }
}
