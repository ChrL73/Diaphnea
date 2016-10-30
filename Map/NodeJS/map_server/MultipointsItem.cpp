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

    void MultipointsItem::addPoint(unsigned int resolutionIndex, double samplingLength, Point *point)
    {
        std::map<double, std::vector<Point *> >::iterator it = _pointVectorMap.find(samplingLength);
        if (it == _pointVectorMap.end())
        {
            it = _pointVectorMap.insert(std::pair<double, std::vector<Point *> >(samplingLength, std::vector<Point *>())).first;
        }
        (*it).second.push_back(point);

        while (_pointVectorVector.size() <= resolutionIndex) _pointVectorVector.push_back(std::vector<Point *>());
        _pointVectorVector[resolutionIndex].push_back(point);

        if (point->getX() < _xMin) _xMin = point->getX();
        if (point->getX() > _xMax) _xMax = point->getX();
        if (point->getY() < _yMin) _yMin = point->getY();
        if (point->getY() > _yMax) _yMax = point->getY();
    }

    void MultipointsItem::addPointArray(std::stringstream& s, int resolutionIndex)
    {
        int i, n = _pointVectorVector[resolutionIndex].size();
        for (i = 0; i < n; ++i)
        {
            if (i != 0) s << ",";
            Point *point =  _pointVectorVector[resolutionIndex][i];
            s << "{\"x\":" << point->getX() << ",\"y\":" << point->getY() << "}";
        }
    }
}
