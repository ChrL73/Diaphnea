#pragma once

#include "Request.h"

namespace map_server
{
    class GetItemInfoRequest : public Request
    {
    private:
        const char * const _mapId;
        const int _itemId;
        const int _resolutionIndex;

        void execute(void);

    public:
        GetItemInfoRequest(const char *socketId, const char *requestId, const char *mapId, int itemId, int resolutionIndex, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _itemId(itemId), _resolutionIndex(resolutionIndex) {}
    };
}

