#include "RenderRequest.h"
#include "MapData.h"
#include "Map.h"
#include "RequestTypeEnum.h"
#include "PolygonElement.h"

#include <map>

namespace map_server
{
    void RenderRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        Map *map = mapData->getMap(_mapId);
        if (map != 0 && _sendResponse)
        {
            std::vector<MapElement *> elementVector;
            unsigned int i, n = _elementIds.size();
            for (i = 0; i < n; ++i)
            {
                MapElement *element = map->getElement(_elementIds[i]);
                if (element != 0) elementVector.push_back(element);
            }

            std::map<LineItem *, std::multimap<int, PolygonElement *> > itemMap;

            MapData::unlock();
        }
        else
        {
            MapData::unlock();
        }
    }
}
