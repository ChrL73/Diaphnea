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
    class MapElement;

    class Map
    {
    private:
        const mongo::OID _mongoId;
        const std::string _id;

        mongo::DBClientConnection * const _connectionPtr;
        bool _loaded;
        std::string _emptyString;

        std::map<std::string, std::string> _languageNameMap;
        std::vector<std::string> _languageIdVector;
        std::map<std::string, std::string> _nameMap;
        std::vector<double> _sampleLengthVector;

        std::map<std::string, MapElement *> _elementMap;

        std::string _infoJson;

    public:
        Map(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr) :
            _mongoId(mongoId), _id(id), _connectionPtr(connectionPtr), _loaded(false) {}
		~Map();

        const std::string& getId(void) const { return _id; }
        bool isLoaded(void) const { return _loaded; }
        void load(void);

        const std::string& getInfoJson(void) const { return _infoJson; }
        MapElement *getElement(const std::string& id);
    };
}
