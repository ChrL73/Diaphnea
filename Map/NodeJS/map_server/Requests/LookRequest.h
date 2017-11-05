#pragma once

#include "Request.h"

namespace map_server
{
    // Note: 'Look' request is no more usefull, because all look information is now sent in the 'MapInfo' response
    class LookRequest : public Request
    {
    private:
        const char * const _mapId;
        const int _lookId;

        void execute(void);

    public:
        LookRequest(const char *socketId, const char *requestId, const char *mapId, int lookId) :
            Request(socketId, requestId), _mapId(mapId), _lookId(lookId) {}
    };
}

