#include "ElementInfoRequest.h"
#include "CommonData.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"

#include <iostream>

namespace map_server
{
    void ElementInfoRequest::execute()
    {
        const char *info = _commonData->getElementInfoJson(_elementId);

        if (info != 0)
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ELEMENT_INFO << " " << info << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"Unknown element id ('" << _elementId << "') in ELEMENT_INFO request\"}" << std::endl;
            _coutMutexPtr->unlock();
        }
    }
}
