#include "Request.h"
#include "RequestTypeEnum.h"
#include "GetMapIdsRequest.h"
#include "GetMapInfoRequest.h"
#include "GetElementInfoRequest.h"
#include "GetElementsInfoRequest.h"

#include <cstdlib>

namespace map_server
{
    std::mutex *Request::_coutMutexPtr = 0;

    Request *Request::createRequest(std::vector<const char *> tokenVector, bool sendResponse)
    {
        if (tokenVector.size() < 3) return 0;
        RequestTypeEnum requestType = static_cast<RequestTypeEnum>(atoi(tokenVector[2]));

        if (requestType == map_server::GET_MAP_IDS)
        {
            return new GetMapIdsRequest(tokenVector[0], tokenVector[1], sendResponse);
        }
        else if (requestType == map_server::GET_MAP_INFO)
        {
            if (tokenVector.size() < 4) return 0;
            return new GetMapInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], sendResponse);
        }
        else if (requestType == map_server::GET_ELEMENT_INFO)
        {
            if (tokenVector.size() < 5) return 0;
            return new GetElementInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4], sendResponse);
        }
        else if (requestType == map_server::GET_ELEMENTS_INFO)
        {
            int i, n = tokenVector.size();
            if (n < 5) return 0;
            std::vector<const char *> elementIds;
            for (i = 4; i < n; ++i) elementIds.push_back(tokenVector[i]);
            return new GetElementsInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], elementIds, sendResponse);
        }

        return 0;
    }
}
