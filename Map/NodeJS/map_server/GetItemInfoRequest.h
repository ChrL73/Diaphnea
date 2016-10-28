#pragma once

#include "Request.h"

namespace map_server
{
    class GetItemInfoRequest : public Request
    {
    private:
        const char * const _mapId;
        int _itemId;

        void execute(void);

    public:
        GetItemInfoRequest(const char *socketId, const char *requestId, const char *mapId, int itemId, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _itemId(itemId) {}
    };
}

