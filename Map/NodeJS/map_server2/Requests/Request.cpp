/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#include "Request.h"
#include "MessageTypeEnum.h"
#include "MapInfoRequest.h"
#include "ElementInfoRequest.h"
#include "ElementsInfoRequest.h"
#include "ItemDataRequest.h"
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
            if (requestType == map_server::MAP_INFO)
            {
                if (tokenCount < 4)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
                        << ",\"message\":\"Not enough tokens in MAP_INFO request (3 tokens, 4 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }
                return new MapInfoRequest(tokenVector[0], tokenVector[1]);
            }
            else if (requestType == map_server::ELEMENT_INFO)
            {
                if (tokenCount < 5)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
                        << ",\"message\":\"Not enough tokens in ELEMENT_INFO request (" << tokenCount << " tokens, 5 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }
                return new ElementInfoRequest(commonData, tokenVector[0], tokenVector[1], tokenVector[4]);
            }
            else if (requestType == map_server::ELEMENTS_INFO)
            {
                if (tokenCount < 5)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
                        << ",\"message\":\"Not enough tokens in ELEMENTS_INFO request (" << tokenCount << " tokens, 5 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                std::vector<const char *> elementIds;
                int i;
                for (i = 4; i < tokenCount; ++i) elementIds.push_back(tokenVector[i]);
                return new ElementsInfoRequest(commonData, tokenVector[0], tokenVector[1], elementIds);
            }
            else if (requestType == map_server::ITEM_DATA)
            {
                if (tokenCount < 6)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
                        << ",\"message\":\"Not enough tokens in ITEM_DATA request (" << tokenCount << " tokens, 6 expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                int itemId;
                try
                {
                    itemId = std::stoi(tokenVector[4]);
                }
                catch (...)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'itemId' parameter ('" << tokenVector[4] << "') in ITEM_DATA request (int expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                int resolutionIndex;
                try
                {
                    resolutionIndex = std::stoi(tokenVector[5]);
                }
                catch (...)
                {
                    _coutMutexPtr->lock();
                    std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
                        << ",\"message\":\"Incorrect 'resolutionIndex' parameter ('" << tokenVector[5] << "') in ITEM_DATA request (int expected)\"}" << std::endl;
                    _coutMutexPtr->unlock();
                    return 0;
                }

                return new ItemDataRequest(tokenVector[0], tokenVector[1], itemId, resolutionIndex);
            }
            else if (requestType == map_server::RENDER)
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
