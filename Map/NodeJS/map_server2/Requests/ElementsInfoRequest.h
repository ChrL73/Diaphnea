#pragma once

#include <vector>

#include "Request.h"

namespace map_server
{
    class ElementsInfoRequest : public Request
    {
    private:
        const std::vector<const char *> _elementIds;

        void execute(void);

    public:
        ElementsInfoRequest(const char *socketId, const char *requestId, const std::vector<const char *>& elementIds) :
            Request(socketId, requestId), _elementIds(elementIds) {}
    };
}

