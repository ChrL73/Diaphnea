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
