#pragma once

#include "Request.h"

namespace map_server
{
    class CommonData;

    class ElementInfoRequest : public Request
    {
    private:
        CommonData * const _commonData;
        const char * const _elementId;

        void execute(void);

    public:
        ElementInfoRequest(CommonData *commonData, const char *socketId, const char *requestId, const char *elementId) :
            Request(socketId, requestId), _commonData(commonData), _elementId(elementId) {}
    };
}
