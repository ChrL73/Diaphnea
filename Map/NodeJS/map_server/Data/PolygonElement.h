#pragma once

#include "MapElement.h"

#include <vector>
#include <string>

namespace map_server
{
    class FilledPolygonItem;
    class PolygonLook;

    class PolygonElement : public MapElement
    {
    private:
        std::vector<const PolygonLook *> _lookVector;
        FilledPolygonItem *_filledPolygonItem;
        std::vector<LineItem *> _lineItemVector;
        std::vector<std::string> _coveredElementVector;

        void load(void);
        bool allowMultiline(void) { return true; }

    public:
        PolygonElement(const std::string& dbName, const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(dbName, mongoId, id, iMap) {}

        const PolygonLook *getLook(int i) const;
        FilledPolygonItem *getFilledPolygonItem(void) const { return _filledPolygonItem; }
        const std::vector<LineItem *>& getLineItemVector(void) const { return _lineItemVector; }
        const std::vector<std::string>& getCoveredElementVector(void) const { return _coveredElementVector; }

    };
}

