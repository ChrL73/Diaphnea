#pragma once

#include "ElementTypeEnum.h"

#include <map>
#include <string>

namespace map_server
{
    class ElementInfo;
    class PointElement;
    class LineElement;
    class PolygonElement;

    class CommonData
    {
    private:
        std::map<std::string, ElementInfo *> _elementInfoMap;
        std::map<std::string, int> _languageIdMap;

        int _lastElementOffset;

    public:
        CommonData(void);
        ~CommonData();

        ElementTypeEnum getElementType(const std::string& elementId);
        const PointElement *getLastElementAsPoint(void) const;
        const LineElement *getLastElementAsLine(void) const;
        const PolygonElement *getLastElementAsPolygon(void) const;

        int getLanguageIndex(const std::string& languageId) const;
    };
}
