#pragma once

#include "Request.h"

namespace map_server
{
    class ItemDataRequest : public Request
    {
    private:
        const int _itemId;
        const int _resolutionIndex;

        void execute(void);

    public:
        ItemDataRequest(const char *socketId, const char *requestId, int itemId, int resolutionIndex) :
            Request(socketId, requestId), _itemId(itemId), _resolutionIndex(resolutionIndex) {}
    };
}

