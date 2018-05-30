#include "Request.h"
#include "MessageTypeEnum.h"
//#include "MapIdsRequest.h"
//#include "MapInfoRequest.h"
//#include "ElementInfoRequest.h"
//#include "ElementsInfoRequest.h"
//#include "ItemDataRequest.h"
//#include "LookRequest.h"
#include "RenderRequest.h"
#include "ErrorEnum.h"
#include "SvgCustomColor.h"

#include <string>
#include <iostream>
#include <cstring>
#include <set>
#include <map>

namespace map_server
{
    std::mutex *Request::_coutMutexPtr = 0;

    Request *Request::createRequest(CommonData *commonData, std::vector<const char *> tokenVector)
    {
        int tokenCount = tokenVector.size();

        if (tokenCount < 3)
        {
            _coutMutexPtr->lock();
            if (tokenCount == 0)
            {
                std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::EMPTY_REQUEST << ",\"message\":\"Empty request\"}" << std::endl;
            }
            else if (tokenCount == 1)
            {
                std::cout << tokenVector[0]  << " -1 " << map_server::ERROR_ << " {\"error\":" << map_server::NO_REQUEST_ID << ",\"message\":\"No Request id\"}" << std::endl;
            }
            else // tokenCount == 2
            {
                std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NO_REQUEST_TYPE << ",\"message\":\"No Request type\"}" << std::endl;
            }
            _coutMutexPtr->unlock();

            return 0;
        }

        bool requestTypeOk = false;
        MessageTypeEnum requestType = MessageTypeEnum::ERROR_;

        try
        {
            requestType = static_cast<MessageTypeEnum>(std::stoi(tokenVector[2]));
            requestTypeOk = true;
        }
        catch (...)
        {
        }

        if (requestTypeOk)
        {
            if (requestType == map_server::RENDER)
            {
                int i;
                if (tokenCount < 12)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
                        << ",\"message\":\"Not enough tokens in RENDER request (" << tokenCount << " tokens, 12 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                double widthInPixels;
                try
                {
                    widthInPixels = std::stod(tokenVector[5]);
                }
                catch (...)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'widthInPixels' parameter ('" << tokenVector[5] << "') in RENDER request (int expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                if (widthInPixels < 1.0 || widthInPixels >= 10000.0)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'widthInPixels' parameter ('" << tokenVector[5] << "') in RENDER request (1 <= widthInPixels < 10000 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                double heightInPixels;
                try
                {
                    heightInPixels = std::stod(tokenVector[6]);
                }
                catch (...)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'heightInPixels' parameter ('" << tokenVector[6] << "') in RENDER request (int expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                if (heightInPixels < 1.0 || heightInPixels >= 10000.0)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'heightInPixels' parameter ('" << tokenVector[6] << "') in RENDER request (1 <= heightInPixels < 10000 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                int lookIndex;
                try
                {
                    lookIndex = std::stoi(tokenVector[7]);
                }
                catch (...)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'lookIndex' parameter ('" << tokenVector[7] << "') in RENDER request (int expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                if (lookIndex < 0)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'lookIndex' parameter ('" << tokenVector[7] << "') in RENDER request (index must be positive or zero)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                std::vector<const char *> elementIds;
                std::set<std::string> framingExceptions;
                for (i = 11; i < tokenCount; ++i)
                {
                    if (strcmp(tokenVector[i], "#") == 0) break;
                    if (tokenVector[i][0] != '-')
                    {
                        elementIds.push_back(tokenVector[i]);
                    }
                    else
                    {
                        elementIds.push_back(tokenVector[i] + 1);
                        framingExceptions.insert(tokenVector[i] + 1);
                    }
                }

                std::map<int, SvgCustomColor *> customColorMap;
                int i0 = i + 1;
                for (i = i0; i + 2 < tokenCount; i += 3)
                {
                    int colorId;
                    try
                    {
                        colorId = std::stoi(tokenVector[i]);
                    }
                    catch (...)
                    {
                        continue;
                    }

                    SvgCustomColor *svgCustomColor = new SvgCustomColor(tokenVector[i + 1], tokenVector[i + 2]);
                    if (svgCustomColor->ok())
                    {
                        customColorMap.insert(std::pair<int, SvgCustomColor *>(colorId, svgCustomColor));
                    }
                    else
                    {
                        delete svgCustomColor;
                    }
                }

                double scale, xFocus, yFocus;
                try
                {
                    scale = std::stod(tokenVector[8]);
                    xFocus = std::stod(tokenVector[9]);
                    yFocus = std::stod(tokenVector[10]);
                }
                catch (...)
                {
                    int framingLevel;
                    try
                    {
                        framingLevel = std::stoi(tokenVector[8]);
                    }
                    catch (...)
                    {
                        framingLevel = 0;
                    }

                    return new RenderRequest(commonData, tokenVector[0], tokenVector[1], tokenVector[4], widthInPixels, heightInPixels,
                                             lookIndex, elementIds, customColorMap, framingLevel, framingExceptions);
                }


                return new RenderRequest(commonData, tokenVector[0], tokenVector[1], tokenVector[4], widthInPixels, heightInPixels, lookIndex, elementIds,
                                         customColorMap, scale, xFocus, yFocus);
            }
        }

        _coutMutexPtr->lock();
        std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_REQUEST_TYPE
            << ",\"message\":\"Incorrect request type: " << tokenVector[2] << "\"}" << std::endl;
        _coutMutexPtr->unlock();

        return 0;
    }
}
