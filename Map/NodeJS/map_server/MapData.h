#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

#include <map>
#include <string>

#include <mutex>

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
        static std::mutex _mutex;

        std::string _mapIds;
        std::map<std::string, Map *> _mapMap;

    public:
        static MapData *instance(void);
        static int destroyInstance(void);

        static void lock(void) { _mutex.lock(); }
        static void unlock(void) { _mutex.unlock(); }

        const Map *getMap(const std::string& id);
        const std::string& getMapIds(void) const { return _mapIds; }
    };
}

