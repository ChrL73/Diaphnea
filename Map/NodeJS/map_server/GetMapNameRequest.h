#pragma once

#include "Request.h"

namespace map_server
{
    class GetMapNameRequest : public Request
    {
    private:
        const char * const _mapId;
        const char * const _languageId;

        void execute(void);

    public:
        GetMapNameRequest(const char *socketId, const char *requestId, const char *mapId, const char *languageId) :
            Request(socketId, requestId), _mapId(mapId), _languageId(languageId) {}
    };
}
