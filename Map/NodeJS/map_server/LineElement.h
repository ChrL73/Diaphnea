#pragma once

#include "MapElement.h"

namespace map_server
{
    class LineElement : public MapElement
    {
    private:
        void load(void);

    public:
        LineElement(const mongo::OID& mongoId, const std::string& id) :
            MapElement(mongoId, id) {}
    };
}

