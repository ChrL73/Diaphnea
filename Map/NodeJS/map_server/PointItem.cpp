#include "PointItem.h"
#include "Point.h"

namespace map_server
{
	PointItem::~PointItem()
	{
		delete _point;
	}
}
