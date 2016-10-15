#include "Request.h"
#include "RequestTypeEnum.h"
#include "GetMapIdsRequest.h"
#include "GetMapNameRequest.h"

#include <cstdlib>

namespace map_server
{
    Request *Request::createRequest(std::vector<const char *> tokenVector)
    {
        if (tokenVector.size() < 3) return 0;
        RequestTypeEnum requestType = static_cast<RequestTypeEnum>(atoi(tokenVector[2]));

        if (requestType == map_server::GET_MAP_IDS)
        {
            return new GetMapIdsRequest(tokenVector[0], tokenVector[1]);
        }
        else if (requestType == map_server::GET_MAP_NAME)
        {
            if (tokenVector.size() < 5) return 0;
            return new GetMapNameRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4]);
        }

        return 0;
    }
}
