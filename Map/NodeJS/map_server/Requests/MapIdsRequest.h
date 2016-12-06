#pragma once

#include "Request.h"

namespace map_server
{
    class MapIdsRequest : public Request
    {
    private:
        void execute(void);

    public:
        MapIdsRequest(const char *socketId, const char *requestId, bool sendResponse) :
            Request(socketId, requestId, sendResponse) {}
    };
}
