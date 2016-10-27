#pragma once

#include "MapElement.h"

#include <vector>

namespace map_server
{
    class FilledPolygonItem;
    class PolygonLook;

    class PolygonElement : public MapElement
    {
    private:
        FilledPolygonItem *_filledPolygonItem;
        std::vector<LineItem *> _lineItemVector;
        const PolygonLook *_look;

        void load(void);

    public:
        PolygonElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}
    };
}

