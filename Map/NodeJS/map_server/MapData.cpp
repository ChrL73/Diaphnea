#include "MapData.h"
#include "Map.h"

namespace map_server
{
    MapData *MapData::_instance = 0;
    bool MapData::_deleteOk = true;

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
    }

    MapData::~MapData()
    {
        _deleteOk = true;

        std::map<std::string, const Map *>::iterator mapIt = _mapMap.begin();
        for (; mapIt != _mapMap.end(); ++mapIt) delete (*mapIt).second;

        mongo::Status status = mongo::client::shutdown();
        if (!status.isOK())
        {
            std::cerr << "Error in mongo::client::shutdown()" << std::endl;
            _deleteOk = false;
        }
    }

    const std::vector<std::string>& MapData::getMapIdVector()
    {
        if (_mapIdVector.empty())
        {

        }

        return _mapIdVector;
    }

    const Map *MapData::getMap(const std::string& id)
    {
        return 0;
    }
}
