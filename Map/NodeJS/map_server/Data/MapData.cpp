#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"

namespace map_server
{
    MapData *MapData::_instance = 0;
    bool MapData::_deleteOk = true;
    std::mutex MapData::_mutex;

    MapData *MapData::instance(void)
    {
        if (_instance == 0)
        {
            _instance = new MapData();
            if (!_instance->_initOk)
            {
                delete _instance;
                _instance = 0;
            }
        }
        return _instance;
    }

    int MapData::destroyInstance(void)
    {
        delete _instance;
        _instance = 0;
        if (!_deleteOk) return -1;
        return 0;
    }

    MapData::MapData(void) : _initOk(true)
    {
        mongo::Status status = mongo::client::initialize();
        if (!status.isOK())
        {
            std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::MONGO_CLIENT_INIT_FAILED << ",\"message\":\"Failed to intialize Mongo client\"}" << std::endl;
            _initOk = false;
            return;
        }

        try
        {
            _connection.connect("localhost");
        }
        catch (const mongo::DBException& e)
        {
            std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::DATABASE_SERVER_CONNECTION_FAILED << ",\"message\":\"Failed to connect to the Mongo database server\"}" << std::endl;
            _initOk = false;
            return;
        }

        _mapIdsJson = "[\"";
        mongo::BSONObj projection = BSON("map" << 1);
		std::unique_ptr<mongo::DBClientCursor> cursor = _connection.query("diaphnea.maps", mongo::BSONObj(), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbMap = cursor->next();

            mongo::BSONElement idElt = dbMap.getField("_id");
            std::string mapId = dbMap.getStringField("map");

            if (!mapId.empty() && idElt.type() == mongo::jstOID)
            {
                if (!_mapMap.empty()) _mapIdsJson += "\",\"";
                Map *map = new Map(idElt.OID(), mapId, &_connection);
                _mapMap.insert(std::pair<std::string, Map *>(map->getId(), map));
                _mapIdsJson += map->getId();
            }
            else
            {
                std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_DATABASE_CONTENT
                    << ",\"message\":\"Unexpected database content, file " << __FILE__ << ", function "
                    << __func__ << ", line " << __LINE__ << "\"}" << std::endl;
            }
        }
        _mapIdsJson += "\"]";

        if (_mapMap.empty())
        {
            std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_DATABASE_CONTENT
                << ",\"message\":\"Unexpected database content, file " << __FILE__ << ", function "
                << __func__ << ", line " << __LINE__ << "\"}" << std::endl;
        }
    }

    MapData::~MapData()
    {
        _deleteOk = true;

        std::map<std::string, Map *>::iterator mapIt = _mapMap.begin();
        for (; mapIt != _mapMap.end(); ++mapIt) delete (*mapIt).second;

        mongo::Status status = mongo::client::shutdown();
        if (!status.isOK())
        {
            std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::MONGO_CLIENT_SHUTDOWN_FAILED << ",\"message\":\"Error during Mongo client shutdown\"}" << std::endl;
            _deleteOk = false;
        }
    }

    Map *MapData::getMap(const std::string& id)
    {
        std::map<std::string, Map *>::iterator mapIt = _mapMap.find(id);
        if (mapIt == _mapMap.end()) return 0;

        Map *map = (*mapIt).second;
        if (!map->isLoaded()) map->load();
        return map;
    }
}
