#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

namespace map_server
{
    class MapElement
    {
    protected:
        const mongo::OID _mongoId;
        const std::string _id;
        bool _loaded;

    public:
        MapElement(const mongo::OID& mongoId, const std::string& id) : _mongoId(mongoId), _id(id), _loaded(false) {}
        virtual ~MapElement() {}

        const std::string& getId(void) const { return _id; }
        bool isLoaded(void) const { return _loaded; }
        virtual void load(void) = 0;
    };
}
