#include "RenderRequest.h"
#include "MapData.h"
#include "Map.h"
#include "MessageTypeEnum.h"
#include "PolygonElement.h"
#include "PointElement.h"
#include "LineElement.h"
#include "PointItem.h"
#include "FilledPolygonItem.h"
#include "LineItem.h"
#include "PolygonLook.h"
#include "ItemLook.h"

#include <map>
#include <sstream>

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

            std::map<LineItem *, std::map<int, PolygonElement *> > lineItemMap;
            std::vector<MapItem *> itemVector;

            n = elementVector.size();
            for (i = 0; i < n; ++i)
            {
                MapElement *element = elementVector[i];

                PointElement *pointElement = dynamic_cast<PointElement *>(element);
                if (pointElement != 0)
                {
                    itemVector.push_back(pointElement->getItem());
                    continue;
                }

                LineElement *lineElement = dynamic_cast<LineElement *>(element);
                if (lineElement != 0)
                {
                    // Todo: handle this case
                    continue;
                }

                PolygonElement *polygonElement = dynamic_cast<PolygonElement *>(element);
                if (polygonElement != 0)
                {
                    itemVector.push_back(polygonElement->getFilledPolygonItem());
                    int j, m = polygonElement->getLineItemVector().size();
                    for (j = 0; j < m; ++j)
                    {
                        LineItem *lineItem = polygonElement->getLineItemVector()[j];
                        std::map<LineItem *, std::map<int, PolygonElement *> >::iterator lineItemIt = lineItemMap.find(lineItem);
                        if (lineItemIt == lineItemMap.end())
                        {
                            lineItemIt = lineItemMap.insert(std::pair<LineItem *, std::map<int, PolygonElement *> >(lineItem, std::map<int, PolygonElement *>())).first;
                        }

                        int zIndex = polygonElement->getLook()->getContourLook()->getZIndex();
                        (*lineItemIt).second.insert(std::pair<int, PolygonElement *>(zIndex, polygonElement));
                    }
                }
            }

            std::map<LineItem *, std::map<int, PolygonElement *> >::iterator lineItemIt = lineItemMap.begin();
            for (; lineItemIt != lineItemMap.end(); ++lineItemIt)
            {
                PolygonElement *element = (*(*lineItemIt).second.begin()).second;
                LineItem *item = (*lineItemIt).first;
                item->setCurrentLook(element->getLook()->getContourLook());
                itemVector.push_back(item);
            }

            n = itemVector.size();
            std::stringstream response;

            if (n > 0)
            {
                response << _socketId << " " << _requestId << " " << map_server::RENDER << " {\"items\":[";
                for (i = 0; i < n; ++i)
                {
                    MapItem *item = itemVector[i];
                    if (i != 0) response << ",";
                    response << "{\"id\":" << item->getId() << ",\"lk\":" << item->getCurrentLook()->getId() << "}";
                }
                response << "],\"resolution\":4}";
            }

            MapData::unlock();

            if (n > 0)
            {
                _coutMutexPtr->lock();
                std::cout << response.str() << std::endl;
                _coutMutexPtr->unlock();
            }
        }
        else
        {
            MapData::unlock();
        }
    }
}
