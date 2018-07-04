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
    class PointLook;
    class NameTranslation;

    class PointElement
    {
    private:
        // String
        int _elementId;

        // Int
        int _itemId;

        // Double
        double _x;

        // Double
        double _y;

        // Double
        double _importance;

        // Array of PointLook
        int _lookCount;
        int _looks;

        // Int
        int _framingLevel;

        // Array of NameTranslation (array size is 'languageCount' defined in 'MapData.cpp')
        int _translations;

        // String
        int _infoJson;

    public:
        static const PointElement *get(int offset) { return reinterpret_cast<const PointElement *>(pointElements + offset); }

        const char *getElementId(void) const { return reinterpret_cast<const char *>(strings) + _elementId; }
        int getItemId(void) const { return _itemId; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getImportance(void) const { return _importance; }

        int getLookCount(void) const { return _lookCount; }
        const PointLook *getLook(int i) const;

        int getFramingLevel(void) const { return _framingLevel; }

        const NameTranslation *getTranslation(int languageIndex) const { return reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex)); }
        const char *getInfoJson(void) const { return reinterpret_cast<const char *>(strings) + _infoJson; }
    };
}

