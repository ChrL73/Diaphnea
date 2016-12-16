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
#include "ItemCopyBuilder.h"
#include "TextDisplayer.h"
#include "PointItemCopy.h"
#include "RepulsiveCenter.h"
#include "Point.h"

#include <map>
#include <sstream>
#include <limits>
#include <cmath>

#if _WIN32
#undef max
#endif

namespace map_server
{
    void RenderRequest::execute()
    {
        MapData::lock();
        MapData *mapData = MapData::instance();

        _map = mapData->getMap(_mapId);
        if (_map != 0 && _sendResponse)
        {
            std::vector<MapElement *> elementVector;
            unsigned int i, n = _elementIds.size();
            for (i = 0; i < n; ++i)
            {
                MapElement *element = _map->getElement(_elementIds[i]);
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
                    int j, m = lineElement->getItemVector().size();
                    for (j = 0; j < m; ++j)
                    {
                        LineItem *lineItem = lineElement->getItemVector()[j];
                        itemVector.push_back(lineItem);
                    }

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

            std::stringstream response;
            n = itemVector.size();

            if (n > 0)
            {
                std::vector<MapItem *> itemVector2;

                if (_focusSetByClient)
                {
                    const double r = 0.6;
                    double dx = r * _widthInPixels / _scale;
                    double xMin = _xFocus - dx;
                    double xMax = _xFocus + dx;
                    double dy = r * _heightInPixels / _scale;
                    double yMin = _yFocus - dy;
                    double yMax = _yFocus + dy;

                    for (i = 0; i < n; ++i)
                    {
                        MapItem *item = itemVector[i];

                        if (item->getXMax() >= xMin && item->getXMin() <= xMax && item->getYMax() >= yMin && item->getYMin() <= yMax)
                        {
                            itemVector2.push_back(item);
                        }
                    }
                }
                else
                {
                    double xMin = std::numeric_limits<double>::max();
                    double xMax = std::numeric_limits<double>::lowest();
                    double yMin = std::numeric_limits<double>::max();
                    double yMax = std::numeric_limits<double>::lowest();

                    for (i = 0; i < n; ++i)
                    {
                        MapItem *item = itemVector[i];
                        if (item->getXMin() < xMin) xMin = item->getXMin();
                        if (item->getXMax() > xMax) xMax = item->getXMax();
                        if (item->getYMin() < yMin) yMin = item->getYMin();
                        if (item->getYMax() > yMax) yMax = item->getYMax();
                        itemVector2.push_back(item);
                    }

                    _xFocus = 0.5 * (xMin + xMax);
                    _yFocus = 0.5 * (yMin + yMax);
                    double geoWidth = 1.15 * (xMax - xMin);
                    double geoHeight = 1.15 * (yMax - yMin);
                    if (geoWidth <= 0) geoWidth = 1.0;
                    if (geoHeight <= 0) geoHeight = 1.0;
                    double geoSize;

                    if (geoWidth * _heightInPixels > geoHeight * _widthInPixels)
                    {
                        double a = _heightInPixels / _widthInPixels;
                        geoSize = geoWidth * sqrt(1.0 + a * a);
                    }
                    else
                    {
                        double a = _widthInPixels / _heightInPixels;
                        geoSize = geoHeight * sqrt(1.0 + a * a);
                    }

                    if (geoSize < _map->getZoomMinDistance()) geoSize = _map->getZoomMinDistance();
                    if (geoSize > _map->getZoomMaxDistance()) geoSize = _map->getZoomMaxDistance();

                    _scale = sqrt(_widthInPixels * _widthInPixels + _heightInPixels * _heightInPixels) / geoSize;
                }

                int resolutionIndex = _map->getResolutionIndex(_scale);

                response << _socketId << " " << _requestId << " " << map_server::RENDER << " {\"items\":[";
                n = itemVector2.size();
                for (i = 0; i < n; ++i)
                {
                    MapItem *item = itemVector2[i];
                    if (i != 0) response << ",";
                    response << "[" << item->getId() << "," << item->getCurrentLook()->getId();
                    if (item->hasResolution()) response << "," << resolutionIndex;
                    response << "]";

                    ItemCopyBuilder *itemCopyBuilder = new ItemCopyBuilder(item, item->getCurrentLook()->getSize(), resolutionIndex);
                    _itemCopyBuilderVector.push_back(itemCopyBuilder);
                }

                response << "],\"xFocus\":" << _xFocus << ",\"yFocus\":" << _yFocus
                         << ",\"scale\":" << _scale << "}";
            }

            MapData::unlock();

            if (n > 0)
            {
                _coutMutexPtr->lock();
                std::cout << response.str() << std::endl;
                _coutMutexPtr->unlock();

                displayText();
            }
        }
        else
        {
            MapData::unlock();
        }
    }

    void RenderRequest::displayText()
    {
        double dx = 0.5 * _widthInPixels / _scale;
        double xMin = _xFocus - dx;
        double xMax = _xFocus + dx;
        double dy = 0.5 * _heightInPixels / _scale;
        double yMin = _yFocus - dy;
        double yMax = _yFocus + dy;

        TextDisplayer textDisplayer(xMin, xMax, yMin, yMax);

        MapData::lock();

        double sizeFactor = _map->getSizeParameter1() / (_map->getSizeParameter2() * _scale);

        int i, n = _itemCopyBuilderVector.size();
        for (i = 0; i < n; ++i)
        {
            ItemCopyBuilder *itemCopyBuilder = _itemCopyBuilderVector[i];
            const MapItem *item = itemCopyBuilder->getItem();

            if (item->getXMax() >= xMin && item->getXMin() <= xMax && item->getYMax() >= yMin && item->getYMin() <= yMax)
            {
                const PointItem *pointItem = dynamic_cast<const PointItem *>(item);
                if (pointItem != 0)
                {
                    PointItemCopy *pointItemCopy = new PointItemCopy();

                    const double coeff = 2.0; // Todo: Check relevance of this value
                    double radius = coeff * itemCopyBuilder->getSize() * sizeFactor;
                    const double u0 = 2.0; // Todo: Check relevance of this value
                    RepulsiveCenter *repulsiveCenter = new RepulsiveCenter(pointItem->getPoint()->getX(), pointItem->getPoint()->getY(),
                                                                           1.0, 0.0, radius, radius, u0, true);
                    pointItemCopy->addRepulsiveCenter(repulsiveCenter);
                    textDisplayer.addItem(pointItemCopy);
                }
            }

            delete itemCopyBuilder;
        }

        MapData::unlock();

        textDisplayer.start();
    }
}
