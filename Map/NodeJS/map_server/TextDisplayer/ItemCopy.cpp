#include "ItemCopy.h"
#include "RepulsiveCenter.h"

namespace map_server
{
	ItemCopy::~ItemCopy()
	{
		int i, n = _repulsiveCenterVector.size();
		for (i = 0; i < n; ++i) delete _repulsiveCenterVector[i];
	}
}
