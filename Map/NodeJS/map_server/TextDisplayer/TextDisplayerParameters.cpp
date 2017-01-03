#include "TextDisplayerParameters.h"

#include <cstring>
#include <cmath>

namespace map_server
{
	bool TextDisplayerParameters::_tablesFilled = false;
	double TextDisplayerParameters::_excludingPotentialTable[_potentialTableSize];
	double TextDisplayerParameters::_notExcludingPotentialTable1[_potentialTableSize];
	double TextDisplayerParameters::_notExcludingPotentialTable2[_potentialTableSize];

	TextDisplayerParameters::TextDisplayerParameters()
	{
		if (!_tablesFilled) fillTables();

		memcpy(_excludingPotentialTableCopy, _excludingPotentialTable, _potentialTableSize * sizeof(double));
		memcpy(_notExcludingPotentialTable1Copy, _notExcludingPotentialTable1, _potentialTableSize * sizeof(double));
		memcpy(_notExcludingPotentialTable2Copy, _notExcludingPotentialTable2, _potentialTableSize * sizeof(double));
	}

	void TextDisplayerParameters::fillTables(void)
	{
		double ratio2 = _maxRepulsionRatio * _maxRepulsionRatio;

		int i;
		for (i = 0; i < _potentialTableSize; ++i)
		{
			double r2 = ratio2 * static_cast<double>(i) / _potentialTableSizeD;
			_excludingPotentialTable[i] = exp(1 - r2);
			_notExcludingPotentialTable2[i] = 2.0 * (ratio2 - r2) / ((ratio2 - 1.0) * (1.0 + r2));

			const double a = 0.4;
			const double b = 0.6;
			if (r2 < a) _notExcludingPotentialTable1[i] = 1.0;
			else if (r2 < b) _notExcludingPotentialTable1[i] = (r2 - b) / (a - b);
			else r2 = 0.0;
		}

		_tablesFilled = true;
	}
}
