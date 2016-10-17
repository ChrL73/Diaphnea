#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

#include <map>
#include <string>

namespace map_server
{
    class Map;

    class MapData
    {
    private:
        static MapData *_instance;
        MapData(void);
        ~MapData();

        int initialize(void);
        int terminate(void);
        bool _initOk;
        static bool _deleteOk;
        mongo::DBClientConnection _connection;

        std::string _mapIds;
        std::map<std::string, Map *> _mapMap;

    public:
        static MapData *instance(void);
        static int destroyInstance(void);

        const Map *getMap(const std::string& id);
        const std::string& getMapIds(void) const { return _mapIds; }
    };
}

