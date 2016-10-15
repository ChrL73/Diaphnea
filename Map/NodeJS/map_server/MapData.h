#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

#include <map>
#include <string>
#include <vector>

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

        std::vector<std::string> _mapIdVector;
        std::map<std::string, const Map *> _mapMap;

    public:
        static MapData *instance(void);
        static int destroyInstance(void);

        const std::vector<std::string>& getMapIdVector();
        const Map *getMap(const std::string& id);
    };
}

