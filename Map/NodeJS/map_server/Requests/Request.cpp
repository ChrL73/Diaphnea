#include "Request.h"
#include "MessageTypeEnum.h"
#include "MapIdsRequest.h"
#include "MapInfoRequest.h"
#include "ElementInfoRequest.h"
#include "ElementsInfoRequest.h"
#include "ItemDataRequest.h"
#include "LookRequest.h"
#include "RenderRequest.h"
#include "ErrorEnum.h"

#include <string>
#include <iostream>

namespace map_server
{
    std::mutex *Request::_coutMutexPtr = 0;

    Request *Request::createRequest(std::vector<const char *> tokenVector)
    {
		MessageTypeEnum requestType = MessageTypeEnum::ERROR_;
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
			if (requestType == map_server::MAP_IDS)
			{
				return new MapIdsRequest(tokenVector[0], tokenVector[1]);
			}
			else if (requestType == map_server::MAP_INFO)
			{
				if (tokenCount < 4)
				{
					_coutMutexPtr->lock();
					std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
						<< ",\"message\":\"Not enough tokens in MAP_ID request (3 tokens, 4 expected)\"}" << std::endl;
					_coutMutexPtr->unlock();
					return 0;
				}
				return new MapInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3]);
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
				return new ElementInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], tokenVector[4]);
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
				return new ElementsInfoRequest(tokenVector[0], tokenVector[1], tokenVector[3], elementIds);
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

				return new ItemDataRequest(tokenVector[0], tokenVector[1], tokenVector[3], itemId, resolutionIndex);
			}
			else if (requestType == map_server::LOOK)
			{
				if (tokenCount < 5)
				{
					_coutMutexPtr->lock();
					std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::NOT_ENOUGH_TOKENS
						<< ",\"message\":\"Not enough tokens in LOOK request (" << tokenCount << " tokens, 5 expected)\"}" << std::endl;
					_coutMutexPtr->unlock();
					return 0;
				}

				int lookId;
				try
				{
					lookId = std::stoi(tokenVector[4]);
				}
				catch (...)
				{
					_coutMutexPtr->lock();
					std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
						<< ",\"message\":\"Incorrect 'lookId' parameter ('" << tokenVector[4] << "') in LOOK request (int expected)\"}" << std::endl;
					_coutMutexPtr->unlock();
					return 0;
				}
				return new LookRequest(tokenVector[0], tokenVector[1], tokenVector[3], lookId);
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

				int lookIndex;
				try
				{
					lookIndex = std::stoi(tokenVector[7]);

					if (lookIndex < 0)
					{
						_coutMutexPtr->lock();
						std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
							<< ",\"message\":\"Incorrect 'lookIndex' parameter ('" << tokenVector[7] << "') in RENDER request (index must be positive or zero)\"}" << std::endl;
						_coutMutexPtr->unlock();
						return 0;
					}
				}
				catch (...)
				{
					_coutMutexPtr->lock();
					std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_PARAMETER
						<< ",\"message\":\"Incorrect 'lookIndex' parameter ('" << tokenVector[7] << "') in RENDER request (int expected)\"}" << std::endl;
					_coutMutexPtr->unlock();
					return 0;
				}

				std::vector<const char *> elementIds;
				for (i = 11; i < tokenCount; ++i) elementIds.push_back(tokenVector[i]);

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
		}

		_coutMutexPtr->lock();
		std::cout << tokenVector[0] << " " << tokenVector[1] << " " << map_server::ERROR_ << " {\"error\":" << map_server::BAD_REQUEST_TYPE
			<< ",\"message\":\"Incorrect request type: " << tokenVector[2] << "\"}" << std::endl;
		_coutMutexPtr->unlock();

        return 0;
    }
}
