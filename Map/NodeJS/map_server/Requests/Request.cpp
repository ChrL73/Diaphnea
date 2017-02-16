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
#include <iostream>

namespace map_server
{
    std::mutex *Request::_coutMutexPtr = 0;

    Request *Request::createRequest(std::vector<const char *> tokenVector)
    {
        if (tokenVector.size() < 3)
        {

            return 0;
        }

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
            return new MapIdsRequest(tokenVector[0], tokenVector[1]);
        }
        else if (requestType == map_server::MAP_INFO)
        {
            if (tokenVector.size() < 4) return 0;
            return new MapInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3]);
        }
        else if (requestType == map_server::ELEMENT_INFO)
        {
            if (tokenVector.size() < 5) return 0;
            return new ElementInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4]);
        }
        else if (requestType == map_server::ELEMENTS_INFO)
        {
            int i, n = tokenVector.size();
            if (n < 5) return 0;
            std::vector<const char *> elementIds;
            for (i = 4; i < n; ++i) elementIds.push_back(tokenVector[i]);
            return new ElementsInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], elementIds);
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
            return new ItemDataRequest(tokenVector[0], tokenVector[1], tokenVector[3], itemId, resolutionIndex);
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
            return new LookRequest(tokenVector[0], tokenVector[1], tokenVector[3], lookId);
        }
        else if (requestType == map_server::RENDER)
        {
            int i, n = tokenVector.size();
            if (n < 12) return 0;

            double widthInPixels, heightInPixels;
            int lookIndex;
            try
            {
                widthInPixels = std::stod(tokenVector[5]);
                heightInPixels = std::stod(tokenVector[6]);
                lookIndex = std::stoi(tokenVector[7]);
                if (lookIndex < 0) return 0;
            }
            catch (...)
            {
                return 0;
            }

            std::vector<const char *> elementIds;
            for (i = 11; i < n; ++i) elementIds.push_back(tokenVector[i]);

            double scale, xFocus, yFocus;
            try
            {
                scale = std::stod(tokenVector[8]);
                xFocus = std::stod(tokenVector[9]);
                yFocus = std::stod(tokenVector[10]);
            }
            catch (...)
            {
                return new RenderRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4], widthInPixels, heightInPixels, lookIndex, elementIds);
            }

            return new RenderRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4], widthInPixels, heightInPixels, lookIndex, elementIds, scale, xFocus, yFocus);
        }

        return 0;
    }
}
