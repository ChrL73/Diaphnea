#include "MultipointsItem.h"
#include "Point.h"

namespace map_server
{
	MultipointsItem::~MultipointsItem()
	{
		std::map<double, std::vector<Point *> >::iterator it = _pointVectorMap.begin();
		for (; it != _pointVectorMap.end(); ++it)
		{
			int i, n = (*it).second.size();
			for (i = 0; i < n; ++i) delete (*it).second[i];
		}
	}

    void MultipointsItem::addPoint(double samplingLength, Point *point)
    {
        std::map<double, std::vector<Point *> >::iterator it = _pointVectorMap.find(samplingLength);
        if (it == _pointVectorMap.end())
        {
            it = _pointVectorMap.insert(std::pair<double, std::vector<Point *> >(samplingLength, std::vector<Point *>())).first;
        }
        (*it).second.push_back(point);
    }
}
