#pragma once

#include "ElementTypeEnum.h"

#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class ElementInfo;
    class PointElement;
    class LineElement;
    class PolygonElement;

    class CommonData
    {
    private:
        std::mutex _mutex;

        std::map<std::string, ElementInfo *> _elementInfoMap;
        std::map<std::string, int> _languageIdMap;

        int _lastElementOffset;

    public:
        CommonData(void);
        ~CommonData();

        void lock(void) { _mutex.lock(); }
        void unlock(void) { _mutex.unlock(); }

        ElementTypeEnum getElementType(const std::string& elementId);
        const PointElement *getLastElementAsPoint(void) const;
        const LineElement *getLastElementAsLine(void) const;
        const PolygonElement *getLastElementAsPolygon(void) const;

        const char *getElementInfoJson(const std::string& elementId);

        int getLanguageIndex(const std::string& languageId) const;
    };
}
