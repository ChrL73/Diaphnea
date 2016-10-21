#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "mongo/client/dbclient.h"

namespace map_server
{
    class MapElement
    {
    protected:
        const mongo::OID _mongoId;
        const std::string _id;

        mongo::DBClientConnection * const _connectionPtr;
        std::vector<std::string> * const _languageIdVectorPtr;
        bool _loaded;
        std::map<std::string, std::string> _nameMap;
        std::string _infoJson;

    public:
        MapElement(const mongo::OID& mongoId, const std::string& id, mongo::DBClientConnection *connectionPtr, std::vector<std::string> *languageIdVectorPtr) :
            _mongoId(mongoId), _id(id), _connectionPtr(connectionPtr), _languageIdVectorPtr(languageIdVectorPtr), _loaded(false) {}
        virtual ~MapElement() {}

        bool isLoaded(void) const { return _loaded; }
        virtual void load(void) = 0;

        const std::string& getId(void) const { return _id; }
        const std::string& getInfoJson(void) const { return _infoJson; }

    };
}
