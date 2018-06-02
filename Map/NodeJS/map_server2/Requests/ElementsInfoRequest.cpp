#include "ElementsInfoRequest.h"
//#include "MapData.h"
//#include "MessageTypeEnum.h"
//#include "ErrorEnum.h"

#include <iostream>

namespace map_server
{
    void ElementsInfoRequest::execute()
    {
        /*std::vector<std::string> infoVector;
        unsigned int i, n = _elementIds.size();
        for (i = 0; i < n; ++i)
        {
            const MapElement *element = map->getElement(_elementIds[i]);
            if (element != 0) infoVector.push_back(element->getInfoJson());
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
        }*/
    }
}
