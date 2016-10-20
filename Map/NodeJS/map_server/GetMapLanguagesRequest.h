#pragma once

#include "Request.h"

namespace map_server
{
    class GetMapLanguagesRequest : public Request
    {
    private:
        const char * const _mapId;

        void execute(void);

    public:
        GetMapLanguagesRequest(const char *socketId, const char *requestId, const char *mapId, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId) {}
    };
}
