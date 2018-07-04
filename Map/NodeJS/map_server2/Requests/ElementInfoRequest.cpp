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
