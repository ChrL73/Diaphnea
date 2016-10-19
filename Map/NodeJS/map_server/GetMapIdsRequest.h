#pragma once

#include "Request.h"

namespace map_server
{
    class GetMapIdsRequest : public Request
    {
    private:
        void execute(void);

    public:
        GetMapIdsRequest(const char *socketId, const char *requestId, bool sendResponse) :
            Request(socketId, requestId, sendResponse) {}
    };
}
