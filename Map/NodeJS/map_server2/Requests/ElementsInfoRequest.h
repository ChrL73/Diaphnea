#pragma once

#include <vector>

#include "Request.h"

namespace map_server
{
    class CommonData;

    class ElementsInfoRequest : public Request
    {
    private:
        CommonData * const _commonData;
        const std::vector<const char *> _elementIds;

        void execute(void);

    public:
        ElementsInfoRequest(CommonData *commonData, const char *socketId, const char *requestId, const std::vector<const char *>& elementIds) :
            Request(socketId, requestId), _commonData(commonData), _elementIds(elementIds) {}
    };
}

