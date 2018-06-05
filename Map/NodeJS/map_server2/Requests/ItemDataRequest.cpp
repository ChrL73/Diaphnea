#include "ItemDataRequest.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"
#include "MapData.h"

#include <iostream>

namespace map_server
{
    void ItemDataRequest::execute()
    {
        int resolutionIndex = _resolutionIndex;
        if (resolutionIndex < 0) resolutionIndex = 0;
        if (resolutionIndex >= sampleLengthCount) resolutionIndex = sampleLengthCount - 1;

        if (_itemId >= 0 && _itemId < itemCount)
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ITEM_DATA << " " << itemData[_itemId * sampleLengthCount + resolutionIndex] << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"Unknown item id ('" << _itemId << "') in ITEM_DATA request\"}" << std::endl;
            _coutMutexPtr->unlock();

        }
    }
}
