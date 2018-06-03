#include "ElementsInfoRequest.h"
#include "CommonData.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"

#include <iostream>

namespace map_server
{
    void ElementsInfoRequest::execute()
    {
        std::vector<const char *> infoVector;
        unsigned int i, n = _elementIds.size();
        for (i = 0; i < n; ++i)
        {
            const char *info = _commonData->getElementInfoJson(_elementIds[i]);
            if (info != 0) infoVector.push_back(info);
            else break;
        }

        if (infoVector.size() == n)
        {
            std::string info = "[";
            for (i = 0; i < n; ++i)
            {
                info += infoVector[i];
                if (i != n-1) info += ",";
                else info += "]";
            }

            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ELEMENTS_INFO << " " << info << std::endl;
            _coutMutexPtr->unlock();
        }
        else
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"Unknown element id ('" << _elementIds[i] << "') in ELEMENTS_INFO request\"}" << std::endl;
            _coutMutexPtr->unlock();
        }
    }
}
