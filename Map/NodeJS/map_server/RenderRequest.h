#pragma once

#include <vector>

#include "Request.h"

namespace map_server
{
    class RenderRequest : public Request
    {
    private:
        const char * const _mapId;
        const double _widthInPixels;
        const double _heightInPixels;
        const std::vector<const char *> _elementIds;

        void execute(void);

    public:
        RenderRequest(const char *socketId, const char *requestId, const char *mapId, double widthInPixels, double heightInPixels,
                      const std::vector<const char *>& elementIds, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _elementIds(elementIds) {}
    };
}


