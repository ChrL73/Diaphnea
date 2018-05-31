#include "MapInfoRequest.h"
#include "MessageTypeEnum.h"
#include "MapData.h"

#include <iostream>

namespace map_server
{
    void MapInfoRequest::execute(void)
    {
        _coutMutexPtr->lock();
        std::cout << _socketId << " " << _requestId << " " << map_server::MAP_INFO << " " << (strings + mapInfo) << std::endl;
        _coutMutexPtr->unlock();
    }
}
