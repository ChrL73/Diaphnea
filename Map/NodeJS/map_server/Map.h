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

        mongo::DBClientConnection * const _connectionPtr;
        bool _loaded;
        std::string _emptyString;

        std::string _languagesJson;
        std::map<std::string, std::string> _languageNameMap;
        std::map<std::string, std::string> _nameMap;
        std::vector<double> _sampleLengthVector;

        std::string _elementIdsJson;
        std::map<std::string, MapElement *> _elementMap;

    public:
        Map(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr) :
            _mongoId(mongoId), _id(id), _connectionPtr(connectionPtr), _loaded(false) {}
		~Map();

        const std::string& getId(void) const { return _id; }
        bool isLoaded(void) const { return _loaded; }
        void load(void);

        const std::string& getName(const std::string& languageId) const;
        const std::string& getLanguagesJson(void) const { return _languagesJson; }
        const std::string& getElementIdsJson(void) const { return _elementIdsJson; }
    };
}
