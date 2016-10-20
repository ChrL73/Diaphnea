#include "Request.h"
#include "RequestTypeEnum.h"
#include "GetMapIdsRequest.h"
#include "GetMapLanguagesRequest.h"
#include "GetMapNameRequest.h"
#include "GetElementIdsRequest.h"

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
        else if (requestType == map_server::GET_MAP_LANGUAGES)
        {
            if (tokenVector.size() < 4) return 0;
            return new GetMapLanguagesRequest(tokenVector[0], tokenVector[1], tokenVector[3], sendResponse);
        }
        else if (requestType == map_server::GET_MAP_NAME)
        {
            if (tokenVector.size() < 5) return 0;
            return new GetMapNameRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4], sendResponse);
        }
        else if (requestType == map_server::GET_ELEMENT_IDS)
        {
            if (tokenVector.size() < 4) return 0;
            return new GetElementIdsRequest(tokenVector[0], tokenVector[1], tokenVector[3], sendResponse);
        }

        return 0;
    }
}
