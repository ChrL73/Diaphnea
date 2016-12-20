#pragma once

#include "IDisplayerConstants.h"

#include <vector>

namespace map_server
{
    class ItemCopy;

    class TextDisplayer : public IDisplayerConstants
    {
    private:
        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        std::vector<ItemCopy *> _itemVector;

        const double _maxRepulsionRatio;
        const int _potentialTableSize;
        const Potential * const _maxPotential;
        const Potential * const _minPotential;

        double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
        int getPotentialTableSize(void) const { return _potentialTableSize; }
        const Potential *getMaxPotential(void) const { return _minPotential; }
        const Potential *getMinPotential(void) const { return _maxPotential; }

    public:
        TextDisplayer(double xMin, double xMax, double yMin, double yMax);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
