#pragma once

#include "MapElement.h"

namespace map_server
{
    class PointElement : public MapElement
    {
    private:
        void load(void);

    public:
        PointElement(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr, std::vector<std::string> *languageIdVectorPtr) :
            MapElement(mongoId, id, connectionPtr, languageIdVectorPtr) {}
    };
}
