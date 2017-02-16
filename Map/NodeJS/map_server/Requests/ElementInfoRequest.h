#pragma once

#include "Request.h"

namespace map_server
{
    class ElementInfoRequest : public Request
    {
    private:
        const char * const _mapId;
        const char * const _elementId;

        void execute(void);

    public:
        ElementInfoRequest(const char *socketId, const char *requestId, const char *mapId, const char *elementId) :
            Request(socketId, requestId), _mapId(mapId), _elementId(elementId) {}
    };
}
