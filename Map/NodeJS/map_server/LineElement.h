#pragma once

#include "MapElement.h"

namespace map_server
{
    class LineLook;

    class LineElement : public MapElement
    {
    private:
        const LineLook *_look;

        void load(void);

    public:
        LineElement(const mongo::OID& mongoId, const std::string& id, IMap *iMap) :
            MapElement(mongoId, id, iMap), _look(0) {}
    };
}

