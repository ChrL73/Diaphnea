#include "Request.h"
#include "MessageTypeEnum.h"
#include "GetMapIdsRequest.h"
#include "GetMapInfoRequest.h"
#include "GetElementInfoRequest.h"
#include "GetElementsInfoRequest.h"
#include "GetItemDataRequest.h"
#include "GetLookRequest.h"
#include "RenderRequest.h"

namespace map_server
{
    std::mutex *Request::_coutMutexPtr = 0;

    Request *Request::createRequest(std::vector<const char *> tokenVector, bool sendResponse)
    {
        if (tokenVector.size() < 3) return 0;

        MessageTypeEnum requestType;
        try
        {
            requestType = static_cast<MessageTypeEnum>(std::stoi(tokenVector[2]));
        }
        catch (...)
        {
            return 0;
        }

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
        else if (requestType == map_server::GET_ITEM_DATA)
        {
            if (tokenVector.size() < 6) return 0;
            int itemId, resolutionIndex;
            try
            {
                itemId = std::stoi(tokenVector[4]);
                resolutionIndex = std::stoi(tokenVector[5]);
            }
            catch (...)
            {
                return 0;
            }
            return new GetItemDataRequest(tokenVector[0], tokenVector[1], tokenVector[3], itemId, resolutionIndex, sendResponse);
        }
        else if (requestType == map_server::GET_LOOK)
        {
            if (tokenVector.size() < 5) return 0;
            int lookId;
            try
            {
                lookId = std::stoi(tokenVector[4]);
            }
            catch (...)
            {
                return 0;
            }
            return new GetLookRequest(tokenVector[0], tokenVector[1], tokenVector[3], lookId, sendResponse);
        }
        else if (requestType == map_server::RENDER)
        {
            int i, n = tokenVector.size();
            if (n < 7) return 0;

            double widthInPixels, heightInPixels;
            try
            {
                widthInPixels = std::stod(tokenVector[4]);
                heightInPixels = std::stod(tokenVector[5]);
            }
            catch (...)
            {
                return 0;
            }

            std::vector<const char *> elementIds;
            for (i = 6; i < n; ++i) elementIds.push_back(tokenVector[i]);
            return new RenderRequest(tokenVector[0], tokenVector[1], tokenVector[3], widthInPixels, heightInPixels, elementIds, sendResponse);
        }

        return 0;
    }
}
