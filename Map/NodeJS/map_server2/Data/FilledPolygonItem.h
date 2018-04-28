#pragma once

namespace map_server
{
    class MultipointItem;

    class FilledPolygonItem
    {
    private:
        const int _itemId;

        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

    public:
        FilledPolygonItem(const MultipointItem *multipointItem);

    };
}
