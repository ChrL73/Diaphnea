 #pragma once

 #include <vector>

namespace map_server
{
    class ItemCopy;

    class TextDisplayer
    {
    private:
        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        std::vector<ItemCopy *> _itemVector;

    public:
        TextDisplayer(double xMin, double xMax, double yMin, double yMax) : _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax) {}
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
