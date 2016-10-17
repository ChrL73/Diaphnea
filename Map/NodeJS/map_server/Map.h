#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

#include <map>
#include <string>

namespace map_server
{
    class MapElement;

    class Map
    {
    private:
        const mongo::OID _mongoId;
        const std::string _id;

        bool _loaded;
        std::map<std::string, MapElement *> _elementMap;

    public:
        Map(const mongo::OID& mongoId, const std::string& id) : _mongoId(mongoId), _id(id), _loaded(false) {}

        const std::string& getId(void) const { return _id; }
        bool isLoaded(void) const { return _loaded; }
        void load(void);
    };
}
