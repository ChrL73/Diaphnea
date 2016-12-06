#include "Request.h"
#include "MessageTypeEnum.h"
#include "MapIdsRequest.h"
#include "MapInfoRequest.h"
#include "ElementInfoRequest.h"
#include "ElementsInfoRequest.h"
#include "ItemDataRequest.h"
#include "LookRequest.h"
#include "RenderRequest.h"

#include <string>

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

        if (requestType == map_server::MAP_IDS)
        {
            return new MapIdsRequest(tokenVector[0], tokenVector[1], sendResponse);
        }
        else if (requestType == map_server::MAP_INFO)
        {
            if (tokenVector.size() < 4) return 0;
            return new MapInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], sendResponse);
        }
        else if (requestType == map_server::ELEMENT_INFO)
        {
            if (tokenVector.size() < 5) return 0;
            return new ElementInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4], sendResponse);
        }
        else if (requestType == map_server::ELEMENTS_INFO)
        {
            int i, n = tokenVector.size();
            if (n < 5) return 0;
            std::vector<const char *> elementIds;
            for (i = 4; i < n; ++i) elementIds.push_back(tokenVector[i]);
            return new ElementsInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], elementIds, sendResponse);
        }
        else if (requestType == map_server::ITEM_DATA)
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
            return new ItemDataRequest(tokenVector[0], tokenVector[1], tokenVector[3], itemId, resolutionIndex, sendResponse);
        }
        else if (requestType == map_server::LOOK)
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
            return new LookRequest(tokenVector[0], tokenVector[1], tokenVector[3], lookId, sendResponse);
        }
        else if (requestType == map_server::RENDER)
        {
            int i, n = tokenVector.size();
            if (n < 10) return 0;

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
            for (i = 9; i < n; ++i) elementIds.push_back(tokenVector[i]);

            double scale, xFocus, yFocus;
            try
            {
                scale = std::stod(tokenVector[6]);
                xFocus = std::stod(tokenVector[7]);
                yFocus = std::stod(tokenVector[8]);
            }
            catch (...)
            {
                return new RenderRequest(tokenVector[0], tokenVector[1], tokenVector[3], widthInPixels, heightInPixels, elementIds, sendResponse);
            }

            return new RenderRequest(tokenVector[0], tokenVector[1], tokenVector[3], widthInPixels, heightInPixels, elementIds, scale, xFocus, yFocus, sendResponse);
        }

        return 0;
    }
}
