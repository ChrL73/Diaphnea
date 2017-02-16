#pragma once

#include "Request.h"

namespace map_server
{
    class MapInfoRequest : public Request
    {
    private:
        const char * const _mapId;

        void execute(void);

    public:
        MapInfoRequest(const char *socketId, const char *requestId, const char *mapId) :
            Request(socketId, requestId), _mapId(mapId) {}
    };
}
