#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"

#include <limits>

namespace map_server
{
    TextDisplayer::TextDisplayer(double xMin, double xMax, double yMin, double yMax) : _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax),
        _maxRepulsionRatio(5.0), _potentialTableSize(10000),
        _maxPotential(new Potential(std::numeric_limits<double>::max(), std::numeric_limits<double>::max())),
        _minPotential(new Potential(-std::numeric_limits<double>::max(), -std::numeric_limits<double>::max()))
    {
    }

	TextDisplayer::~TextDisplayer()
	{
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i) delete _itemVector[i];

		delete _maxPotential;
		delete _minPotential;
	}

    void TextDisplayer::start(void)
    {

    }
}
