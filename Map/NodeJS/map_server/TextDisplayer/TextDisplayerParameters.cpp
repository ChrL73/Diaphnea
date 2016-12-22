#include "TextDisplayerParameters.h"

#include <cstring>
#include <cmath>

namespace map_server
{
	bool TextDisplayerParameters::_tablesFilled = false;
	double TextDisplayerParameters::_excludingPotentialTable[_potentialTableSize];
	double TextDisplayerParameters::_notExcludingPotentialTable[_potentialTableSize];

	TextDisplayerParameters::TextDisplayerParameters()
	{
		if (!_tablesFilled) fillTables();

		memcpy(_excludingPotentialTableCopy, _excludingPotentialTable, _potentialTableSize * sizeof(double));
		memcpy(_notExcludingPotentialTableCopy, _notExcludingPotentialTable, _potentialTableSize * sizeof(double));
	}

	void TextDisplayerParameters::fillTables(void)
	{
		double ratio2 = _maxRepulsionRatio * _maxRepulsionRatio;

		int i;
		for (i = 0; i < _potentialTableSize; ++i)
		{
			double r2 = ratio2 * static_cast<double>(i) / _potentialTableSizeD;
			_excludingPotentialTable[i] = exp(1 - r2);
			_notExcludingPotentialTable[i] = 2.0 * (ratio2 - r2) / ((ratio2 - 1.0) * (1.0 + r2));
		}

		_tablesFilled = true;
	}
}
