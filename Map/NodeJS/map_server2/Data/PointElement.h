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

        // Array of PointLooks
        int _lookCount;
        int _looks;

        // Int
        int _framingLevel;

        // Array of NameTranslations (array size is 'languageCount' defined in 'MapData.cpp')
        int _translations;

    public:
        static const PointElement *get(int offset) { return reinterpret_cast<const PointElement *>(pointElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }
        int getItemId(void) const { return _itemId; }
        double getX(void) const { return _x; }
        double getY(void) const { return _y; }
        double getImportance(void) const { return _importance; }

        int getLookCount(void) const { return _lookCount; }
        const PointLook *getLook(int i) const;

        int getFramingLevel(void) const { return _framingLevel; }

        const NameTranslation *getTranslation(int languageIndex) const { return reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex)); }
    };
}

