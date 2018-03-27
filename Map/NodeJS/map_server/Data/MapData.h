#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#ifdef __GNUC__
// Avoid this warning: ‘template<class> class std::auto_ptr’ is deprecated
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#endif
#include "mongo/client/dbclient.h"
#ifdef __GNUC__
#pragma GCC diagnostic warning "-Wdeprecated-declarations"
#endif

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
        MapData(const std::string& dbHost, const std::string& dbName, const std::string& dbUser, const std::string& dbPassword);
        ~MapData();

        bool _initOk;
        static bool _deleteOk;
        mongo::DBClientConnection _connection;
        static std::mutex _mutex;

        const std::string _dbHost;
        const std::string _dbName;
        const std::string _dbUser;
        const std::string _dbPassword;

        std::string _mapIdsJson;
        std::map<std::string, Map *> _mapMap;

    public:
        static MapData *instance(const std::string& dbHost = "", const std::string& dbName = "", const std::string& dbUser = "", const std::string& dbPassword = "");
        static int destroyInstance(void);

        static void lock(void) { _mutex.lock(); }
        static void unlock(void) { _mutex.unlock(); }

        Map *getMap(const std::string& id);
        const std::string& getMapIdsJson(void) const { return _mapIdsJson; }
    };
}

