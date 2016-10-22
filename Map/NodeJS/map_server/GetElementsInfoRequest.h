#pragma once

#include <vector>

#include "Request.h"

namespace map_server
{
    class GetElementsInfoRequest : public Request
    {
    private:
        const char * const _mapId;
        const std::vector<const char *> _elementIds;

        void execute(void);

    public:
        GetElementsInfoRequest(const char *socketId, const char *requestId, const char *mapId, const std::vector<const char *>& elementIds, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _elementIds(elementIds) {}
    };
}

