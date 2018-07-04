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

#include "MapData.h"

namespace map_server
{
    class MultipointItem;
    class PolygonLook;
    class NameTranslation;

    class PolygonElement
    {
    private:
        // String
        int _elementId;

        // MultipointItem
        int _contour;

        // Array of MultipointItem
        int _itemCount;
        int _items;

        // Array of strings
        int _coveredElementCount;
        int _coveredElements;

        // Double
        double _importance;

        // Array of PolygonLook
        int _lookCount;
        int _looks;

        // Int
        int _framingLevel;

        // Array of NameTranslation (array size is 'languageCount' defined in 'MapData.cpp')
        int _translations;

        // String
        int _infoJson;

    public:
        static const PolygonElement *get(int offset) { return reinterpret_cast<const PolygonElement *>(polygonElements + offset); }

        const char *getElementId(void) const { return reinterpret_cast<const char *>(strings) + _elementId; }

        const MultipointItem *getContour(void) const { return reinterpret_cast<const MultipointItem *>(multipointItems + _contour); }

        int getItemCount(void) const { return _itemCount; }
        const MultipointItem *getItem(int i) const { return reinterpret_cast<const MultipointItem *>(multipointItems + *(intArrays + _items + i)); }

        int getCoveredElementCount(void) const { return _coveredElementCount; }
        const char *getCoveredElement(int i) const { return reinterpret_cast<const char *>(strings) + *(intArrays + _coveredElements + i); }

        double getImportance(void) const { return _importance; }

        int getLookCount(void) const { return _lookCount; }
        const PolygonLook *getLook(int i) const;

        int getFramingLevel(void) const { return _framingLevel; }

        const NameTranslation *getTranslation(int languageIndex) const { return reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex)); }
        const char *getInfoJson(void) const { return reinterpret_cast<const char *>(strings) + _infoJson; }
    };
}
