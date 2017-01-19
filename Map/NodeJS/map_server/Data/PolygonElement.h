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
        const PolygonLook *_look;
        FilledPolygonItem *_filledPolygonItem;
        std::vector<LineItem *> _lineItemVector;
        std::vector<std::string> _coveredElementVector;

        void load(void);

    public:
        PolygonElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}

        const PolygonLook *getLook(void) const { return _look; }
        FilledPolygonItem *getFilledPolygonItem(void) const { return _filledPolygonItem; }
        const std::vector<LineItem *>& getLineItemVector(void) const { return _lineItemVector; }
        const std::vector<std::string>& getCoveredElementVector(void) const { return _coveredElementVector; }

    };
}

