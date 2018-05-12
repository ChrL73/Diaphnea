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

    public:
        static const PolygonElement *get(int offset) { return reinterpret_cast<const PolygonElement *>(polygonElements + offset); }

        const char *getElementId(void) const { return strings + _elementId; }

        const MultipointItem *getContour(void) const { return reinterpret_cast<const MultipointItem *>(multipointItems + _contour); }

        int getItemCount(void) const { return _itemCount; }
        const MultipointItem *getItem(int i) const { return reinterpret_cast<const MultipointItem *>(multipointItems + *(intArrays + _items + i)); }

        int getCoveredElementCount(void) const { return _coveredElementCount; }
        const char *getCoveredElement(int i) const { return strings + *(intArrays + _coveredElements + i); }

        double getImportance(void) const { return _importance; }

        int getLookCount(void) const { return _lookCount; }
        const PolygonLook *getLook(int i) const;

        int getFramingLevel(void) const { return _framingLevel; }

        const NameTranslation *getTranslation(int languageIndex) const { return reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex)); }
    };
}
