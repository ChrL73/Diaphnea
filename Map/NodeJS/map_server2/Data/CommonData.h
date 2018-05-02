#pragma once

#include "ElementTypeEnum.h"

#include <map>
#include <string>
//#include <mutex>

namespace map_server
{
    class ElementInfo;
    class PointElement;
    class LineElement;
    class PolygonElement;

    class CommonData
    {
    private:
        // The mutex does not seem to be necessary, because the memory locations shared between the threads are read, but never modified
        //std::mutex _mutex;
        std::map<std::string, ElementInfo *> _elementInfoMap;

        int _lastElementOffset;

    public:
        CommonData(void);
        ~CommonData();

        //void lock(void) { _mutex.lock(); }
        //void unlock(void) { _mutex.unlock(); }

        ElementTypeEnum getElementType(const std::string& elementId);
        const PointElement *getLastElementAsPoint(void) const;
        const LineElement *getLastElementAsLine(void) const;
        const PolygonElement *getLastElementAsPolygon(void) const;
    };
}
