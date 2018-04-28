#pragma once

namespace map_server
{
    class MultipointItem;

    class LineItem
    {
    private:
        const int _itemId;

        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        int _minZIndex;

    public:
        LineItem(const MultipointItem *multipointItem);

        int getItemId(void) const { return _itemId; }

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
    };
}
