#include "MultipointsItem.h"

namespace map_server
{
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
