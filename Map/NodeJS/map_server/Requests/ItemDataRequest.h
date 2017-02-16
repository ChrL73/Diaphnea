#pragma once

#include "Request.h"

namespace map_server
{
    class ItemDataRequest : public Request
    {
    private:
        const char * const _mapId;
        const int _itemId;
        const int _resolutionIndex;

        void execute(void);

    public:
        ItemDataRequest(const char *socketId, const char *requestId, const char *mapId, int itemId, int resolutionIndex) :
            Request(socketId, requestId), _mapId(mapId), _itemId(itemId), _resolutionIndex(resolutionIndex) {}
    };
}

