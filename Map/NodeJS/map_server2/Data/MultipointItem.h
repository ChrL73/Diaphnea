#pragma once

namespace map_server
{
    class MultipointItem
    {
    private:
        double _xMin;
        double _xMax;
        double _yMin;
        double _yMax;

        int _itemId;

    public:
        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }

        int getItemId(void) const { return _itemId; }

    };
}
