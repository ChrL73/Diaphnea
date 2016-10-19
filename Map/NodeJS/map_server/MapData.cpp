#include "MapData.h"
#include "Map.h"

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
            std::cerr << "Error in mongo::client::initialize()" << std::endl;
            _initOk = false;
            return;
        }

        try
        {
            _connection.connect("localhost");
        }
        catch(const mongo::DBException& e)
        {
            std::cerr << "Exception in _connection.connect(\"localhost\")" << std::endl;
            _initOk = false;
        }

        mongo::BSONObj projection = BSON("map" << 1);
        auto cursor = _connection.query("diaphnea.maps", mongo::BSONObj(), 0, 0, &projection);
        while (cursor->more())
        {
            mongo::BSONObj dbMap = cursor->next();
            Map *map = new Map(dbMap.getField("_id").OID(), dbMap.getStringField("map"), &_connection);
            _mapMap.insert(std::pair<std::string, Map *>(map->getId(), map));
            if (!_mapIds.empty()) _mapIds += " ";
            _mapIds += map->getId();
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
            std::cerr << "Error in mongo::client::shutdown()" << std::endl;
            _deleteOk = false;
        }
    }

    const Map *MapData::getMap(const std::string& id)
    {
        std::map<std::string, Map *>::iterator mapIt = _mapMap.find(id);
        if (mapIt == _mapMap.end()) return 0;

        Map *map = (*mapIt).second;
        if (!map->isLoaded()) map->load();
        return map;

    }
}
