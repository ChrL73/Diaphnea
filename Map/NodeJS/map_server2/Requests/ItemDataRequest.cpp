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
