#pragma once

#include "MapData.h"

namespace map_server
{
    class PointList;

    class MultipointItem
    {
    private:
        double _xMin;
        double _xMax;
        double _yMin;
        double _yMax;

        int _itemId;

        // Array of PointList (array size is 'sampleLengthCount' defined in 'MapData.cpp')
        int _pointLists;

        int _cap1Round;
        int _cap2Round;

    public:
        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }

        int getItemId(void) const { return _itemId; }

        const PointList *getPointList(int i) const { return reinterpret_cast<const PointList *>(pointLists + *(intArrays + _pointLists + i)); }

        bool cap1Round(void) const { return static_cast<bool>(_cap1Round); }
        bool cap2Round(void) const { return static_cast<bool>(_cap2Round); }
    };
}
