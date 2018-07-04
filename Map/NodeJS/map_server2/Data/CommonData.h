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

#pragma once

#include "ElementTypeEnum.h"

#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class ElementInfo;
    class PointElement;
    class LineElement;
    class PolygonElement;

    class CommonData
    {
    private:
        std::mutex _mutex;

        std::map<std::string, ElementInfo *> _elementInfoMap;
        std::map<std::string, int> _languageIdMap;

        int _lastElementOffset;

    public:
        CommonData(void);
        ~CommonData();

        void lock(void) { _mutex.lock(); }
        void unlock(void) { _mutex.unlock(); }

        ElementTypeEnum getElementType(const std::string& elementId);
        const PointElement *getLastElementAsPoint(void) const;
        const LineElement *getLastElementAsLine(void) const;
        const PolygonElement *getLastElementAsPolygon(void) const;

        const char *getElementInfoJson(const std::string& elementId);

        int getLanguageIndex(const std::string& languageId) const;
    };
}
