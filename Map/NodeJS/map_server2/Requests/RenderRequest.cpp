#include "RenderRequest.h"
//#include "MapData.h"
//#include "Map.h"
#include "MessageTypeEnum.h"
#include "PointElement.h"
//#include "LineElement.h"
//#include "PolygonElement.h"
#include "CommonData.h"
#include "PointItem.h"
//#include "FilledPolygonItem.h"
//#include "LineItem.h"
//#include "PolygonLook.h"
//#include "ItemLook.h"
//#include "ItemCopyBuilder.h"
//#include "TextDisplayer.h"
//#include "PointItemCopy.h"
//#include "LineItemCopy.h"
//#include "FilledPolygonItemCopy.h"
//#include "RepulsiveCenter.h"
//#include "Point.h"
//#include "TextInfo.h"
//#include "TextInfoLine.h"
//#include "TextDisplayerParameters.h"
//#include "ElementName.h"
//#include "SvgCreator.h"
//#include "SvgItemInfo.h"
//include "ErrorEnum.h"
#include "SvgCustomColor.h"
//#include "Category.h"
//#include "PointVector.h"

//#include "ft2build.h"
//#include FT_FREETYPE_H

#include <cstring>
//#include <map>
//#include <sstream>
//#include <limits>
//#include <cmath>

#if _WIN32
#undef max
#endif

namespace map_server
{
    RenderRequest::~RenderRequest()
    {
        std::map<int, SvgCustomColor *>::const_iterator it = _customColorMap.begin();
        for (; it != _customColorMap.end(); ++it) delete (*it).second;
    }

    void RenderRequest::execute()
    {
        bool createSvg = (strcmp(_socketId, "svg") == 0);

        std::vector<const PointItem *> pointItemVector;

        _commonData->lock();

        unsigned int i, n = _elementIds.size();
        for (i = 0; i < n; ++i)
        {
            std::string elementId = _elementIds[i];
            ElementTypeEnum elementType = _commonData->getElementType(elementId);

            if (elementType == map_server::POINT)
            {
                const PointElement *pointElement = _commonData->getLastElementAsPoint();
                PointItem *item = new PointItem(pointElement->getItemId(), pointElement->getElementId());
            }
            else if (elementType == map_server::LINE)
            {

            }
            else if (elementType == map_server::POLYGON)
            {

            }
            else if (elementId == "#test")
            {
                //_testMode = true;
            }
            // Disable potential image creation, because it's only a developement/debug/test feature.
            // Moreover, this  feature requires a significant calculation time that could overload the server if used in production.
            /*else if (elementId == "#img")
            {
                _createPotentialImage = true;
            }*/
        }

        _commonData->unlock();

        /*std::map<LineItem *, std::map<int, PolygonElement *> > lineItemMap;
        std::vector<MapItem *> itemVector;
        std::set<std::string> coveredElementSet;

        n = elementVector.size();
        bool noElement = (n == 0);
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

                    int zIndex = polygonElement->getLook(0)->getContourLook()->getZIndex();
                    (*lineItemIt).second.insert(std::pair<int, PolygonElement *>(zIndex, polygonElement));
                }

                m = polygonElement->getCoveredElementVector().size();
                for (j = 0; j < m; ++j) coveredElementSet.insert(polygonElement->getCoveredElementVector()[j]);
            }
        }

        std::map<LineItem *, std::map<int, PolygonElement *> >::iterator lineItemIt = lineItemMap.begin();
        for (; lineItemIt != lineItemMap.end(); ++lineItemIt)
        {
            PolygonElement *element = (*(*lineItemIt).second.begin()).second;
            LineItem *item = (*lineItemIt).first;
            std::vector<const ItemLook *> contourLook;
            contourLook.push_back(element->getLook(_lookIndex)->getContourLook());
            item->setCurrentLooks(contourLook);
            itemVector.push_back(item);
        }

        bool languageOk = _map->knownLanguage(_languageId);
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
                double yMin = xMin;
                double yMax = xMax;
                double xMin0 = xMin;
                double xMax0 = xMax;
                double yMin0 = xMin;
                double yMax0 = xMax;
                bool minMaxSet = false;

                for (i = 0; i < n; ++i)
                {
                    MapItem *item = itemVector[i];
                    itemVector2.push_back(item);

                    int framingLevel = 0;
                    if (item->getElementForFraming() != 0 && _framingExceptions.find(item->getElementForFraming()->getId()) == _framingExceptions.end())
                    {
                        framingLevel = item->getElementForFraming()->getCategory()->getFramingLevel();
                    }

                    if (framingLevel >= _framingLevel)
                    {
                        if (item->getXMin() < xMin) xMin = item->getXMin();
                        if (item->getXMax() > xMax) xMax = item->getXMax();
                        if (item->getYMin() < yMin) yMin = item->getYMin();
                        if (item->getYMax() > yMax) yMax = item->getYMax();
                        minMaxSet = true;
                    }

                    if (!minMaxSet)
                    {
                        if (item->getXMin() < xMin0) xMin0 = item->getXMin();
                        if (item->getXMax() > xMax0) xMax0 = item->getXMax();
                        if (item->getYMin() < yMin0) yMin0 = item->getYMin();
                        if (item->getYMax() > yMax0) yMax0 = item->getYMax();
                    }
                }

                if (!minMaxSet)
                {
                    xMin = xMin0;
                    xMax = xMax0;
                    yMin = yMin0;
                    yMax = yMax0;
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

            std::map<std::string, std::vector<ItemCopyBuilder *> > lineItemAssociationMap;
            double dx = 0.5 * _widthInPixels / _scale;
            double xMin = _xFocus - dx;
            double xMax = _xFocus + dx;
            double dy = 0.5 * _heightInPixels / _scale;
            double yMin = _yFocus - dy;
            double yMax = _yFocus + dy;

            double sizeFactor = _map->getSizeParameter1() / (_map->getSizeParameter2() + _scale);

            if (createSvg) _svgCreator = new SvgCreator(_widthInPixels, _heightInPixels, _scale, sizeFactor, _xFocus, _yFocus, _socketId, _requestId, &_customColorMap);
            else response << _socketId << " " << _requestId << " " << map_server::RENDER << " {\"items\":[";

            n = itemVector2.size();
            for (i = 0; i < n; ++i)
            {
                MapItem *item = itemVector2[i];

                if (_svgCreator == 0)
                {
                    if (i != 0) response << ",";
                    response << "[" << item->getId() << "," << item->getCurrentLook(_lookIndex)->getId();
                    if (item->hasResolution()) response << "," << resolutionIndex;
                    response << "]";
                }
                else
                {
                    SvgItemInfo *svgItemInfo = new SvgItemInfo(item, item->getCurrentLook(_lookIndex), resolutionIndex);
                    _svgCreator->addInfo(item->getCurrentLook(_lookIndex)->getZIndex(), svgItemInfo);
                }

                if (coveredElementSet.find(item->getElementIdForText()) == coveredElementSet.end())
                {
                    if (languageOk)
                    {
                        ItemCopyBuilder *itemCopyBuilder = new ItemCopyBuilder(item, item->getCurrentLook(_lookIndex)->getSize(), item->getCurrentTextLook(_lookIndex), resolutionIndex);
                        _itemCopyBuilderVector.push_back(itemCopyBuilder);

                        LineItem *lineItem = dynamic_cast<LineItem *>(item);
                        if (lineItem != 0 && lineItem->getXMax() >= xMin && lineItem->getXMin() <= xMax && lineItem->getYMax() >= yMin && lineItem->getYMin() <= yMax)
                        {
                            std::string elementId = lineItem->getElementIdForText();
                            if (!elementId.empty())
                            {
                                std::map<std::string, std::vector<ItemCopyBuilder *> >::iterator elementIt = lineItemAssociationMap.find(elementId);
                                if (elementIt == lineItemAssociationMap.end())
                                {
                                    elementIt = lineItemAssociationMap.insert(std::pair<std::string, std::vector<ItemCopyBuilder *> >(elementId, std::vector<ItemCopyBuilder *>())).first;
                                }

                                (*elementIt).second.push_back(itemCopyBuilder);
                            }
                        }
                    }
                }
                else
                {
                    if (_svgCreator == 0)
                    {
                        std::stringstream delResponse;
                        delResponse << _socketId << " " << _requestId << " " << map_server::REMOVE_TEXT
                            << " {\"e\":\"" << item->getElementIdForText() << "\"}";
                        MapData::unlock();
                        _coutMutexPtr->lock();
                        std::cout << delResponse.str() << std::endl;
                        _coutMutexPtr->unlock();
                        MapData::lock();
                    }
                }
            }

            if (_svgCreator == 0)
            {
                response << "],\"xFocus\":" << _xFocus << ",\"yFocus\":" << _yFocus
                         << ",\"scale\":" << _scale << "}";
            }

            std::map<std::string, std::vector<ItemCopyBuilder *> >::iterator elementIt = lineItemAssociationMap.begin();
            for (; elementIt != lineItemAssociationMap.end(); ++elementIt)
            {
                (*elementIt).second[0]->setLineBuilderVector((*elementIt).second);
            }
        }

        MapData::unlock();

        if (n > 0)
        {
            if (_svgCreator == 0)
            {
                _coutMutexPtr->lock();
                std::cout << response.str() << std::endl;
                _coutMutexPtr->unlock();
            }

            if (languageOk)
            {
                displayText();
            }
            else
            {
                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                    << ",\"message\":\"Unknown language id ('" << _languageId << "') in RENDER request\"}" << std::endl;
                _coutMutexPtr->unlock();
            }
        }
        else if (noElement)
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                << ",\"message\":\"All element ids are unknown ('";

            n = _elementIds.size();
            for (i = 0; i < n; ++i)
            {
                std::cout << _elementIds[i];
                if (i != n - 1) std::cout << " ";
            }

            std::cout << "') in RENDER request\"}" << std::endl;
            _coutMutexPtr->unlock();
        }

        delete _svgCreator;

        flushErrors(_map);*/
    }
}
