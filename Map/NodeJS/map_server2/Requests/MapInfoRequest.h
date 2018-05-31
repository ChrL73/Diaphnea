#pragma once

#include "Request.h"

namespace map_server
{
    class MapInfoRequest : public Request
    {
    private:
        void execute(void);

    public:
        MapInfoRequest(const char *socketId, const char *requestId) :
            Request(socketId, requestId) {}
    };
}
