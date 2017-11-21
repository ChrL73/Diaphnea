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
#include "LineItemCopy.h"
#include "FilledPolygonItemCopy.h"
#include "RepulsiveCenter.h"
#include "Point.h"
#include "TextInfo.h"
#include "TextInfoLine.h"
#include "TextDisplayerParameters.h"
#include "ElementName.h"
#include "SvgCreator.h"
#include "SvgItemInfo.h"
#include "ErrorEnum.h"
#include "SvgCustomColor.h"
#include "Category.h"

#include "ft2build.h"
#include FT_FREETYPE_H

#include <map>
#include <sstream>
#include <limits>
#include <cmath>

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
        MapData::lock();
        MapData *mapData = MapData::instance();

        _map = mapData->getMap(_mapId);
        if (_map != 0)
        {
            bool createSvg = (strcmp(_socketId, "svg") == 0);

            std::vector<MapElement *> elementVector;
            unsigned int i, n = _elementIds.size();
            for (i = 0; i < n; ++i)
            {
                std::string elementId = _elementIds[i];
                MapElement *element = _map->getElement(elementId);
                if (element != 0) elementVector.push_back(element);

                // Disable potential image creation, because it's only a developement/debug/test feature.
                // Moreover, this  feature requires a significant calculation time that could overload the server if used in production.
                //else if (elementId == "#img") _createPotentialImage = true;

                else if (elementId == "#test") _testMode = true;
            }

            std::map<LineItem *, std::map<int, PolygonElement *> > lineItemMap;
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
                item->setCurrentCategory(element->getCategory());
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
                    double yMin = std::numeric_limits<double>::max();
                    double yMax = std::numeric_limits<double>::lowest();

                    for (i = 0; i < n; ++i)
                    {
                        MapItem *item = itemVector[i];
                        itemVector2.push_back(item);

                        int framingLevel = 0;
                        if (item->getCurrentCategory() != 0) framingLevel = item->getCurrentCategory()->getFramingLevel();
                        if (framingLevel >= _framingLevel)
                        {
                            if (item->getXMin() < xMin) xMin = item->getXMin();
                            if (item->getXMax() > xMax) xMax = item->getXMax();
                            if (item->getYMin() < yMin) yMin = item->getYMin();
                            if (item->getYMax() > yMax) yMax = item->getYMax();
                        }
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

			flushErrors(_map);
        }
        else
        {
            MapData::unlock();

            _coutMutexPtr->lock();
			std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
				<< ",\"message\":\"Unknown map id ('" << _mapId << "') in RENDER request\"}" << std::endl;
			_coutMutexPtr->unlock();
        }
    }

    bool RenderRequest::displayText()
    {
#ifdef _WIN32
		const std::string fontFile = "arial.ttf";
#else
		const std::string fontFile = "/usr/share/fonts/truetype/msttcorefonts/arial.ttf";
#endif

		FT_Library  library;
		int error = FT_Init_FreeType(&library);
		if (error) return false;

		FT_Face face;
		error = FT_New_Face(library, fontFile.c_str(), 0, &face);
		if (error)
        {
            FT_Done_FreeType(library);
            return false;
        }

		double dx = 0.5 * _widthInPixels / _scale;
        double xMin = _xFocus - dx;
        double xMax = _xFocus + dx;
        double dy = 0.5 * _heightInPixels / _scale;
        double yMin = _yFocus - dy;
        double yMax = _yFocus + dy;
        MapData::lock();

		TextDisplayerParameters parameters;
		TextDisplayer textDisplayer(&parameters, _socketId, _requestId, _widthInPixels, _heightInPixels, _xFocus, _yFocus, _scale, _createPotentialImage, _svgCreator, _testMode);

        double sizeFactor = _map->getSizeParameter1() / (_map->getSizeParameter2() + _scale);

        int i, n = _itemCopyBuilderVector.size();
        for (i = 0; i < n; ++i)
        {
            ItemCopyBuilder *itemCopyBuilder = _itemCopyBuilderVector[i];
            const MapItem *item = itemCopyBuilder->getItem();
			double size = itemCopyBuilder->getSize();
			int resolutionIndex = itemCopyBuilder->getResolutionIndex();

            // Todo: make this test before creating 'ItemCopyBuilder' objects
            if (item->getXMax() >= xMin && item->getXMin() <= xMax && item->getYMax() >= yMin && item->getYMin() <= yMax)
            {
                const PointItem *pointItem = dynamic_cast<const PointItem *>(item);
                if (pointItem != 0)
                {
                    double diameter = size * sizeFactor * _scale;
                    double x = (pointItem->getPoint()->getX() - _xFocus) * _scale + 0.5 * _widthInPixels;
                    double y = (pointItem->getPoint()->getY() - _yFocus) * _scale + 0.5 * _heightInPixels;
                    PointItemCopy *pointItemCopy = new PointItemCopy(pointItem->getElementIdForText(), pointItem->getImportance(), x, y, diameter);
                    double radius = parameters.getPointRadiusCoeff() * diameter;
                    RepulsiveCenter *repulsiveCenter = new RepulsiveCenter(&parameters, x, y, 1.0, 0.0, radius, radius, parameters.getPointRefPotential(), true, false);
                    pointItemCopy->addRepulsiveCenter(repulsiveCenter);
                    setTextInfo(pointItemCopy, itemCopyBuilder, sizeFactor, face);
                    textDisplayer.addItem(pointItemCopy);
                }
				else
				{
					const LineItem *lineItem = dynamic_cast<const LineItem *>(item);
					if (lineItem != 0)
					{
                        LineItemCopy *lineItemCopy = new LineItemCopy(lineItem->getElementIdForText(), lineItem->getImportance());

						int j, m = lineItem->getPointVector(resolutionIndex).size();
						for (j = 0; j < m - 1; ++j)
						{
							const Point *point1 = lineItem->getPointVector(resolutionIndex)[j];
							const Point *point2 = lineItem->getPointVector(resolutionIndex)[j + 1];
							double x1 = (point1->getX() - _xFocus) * _scale + 0.5 * _widthInPixels;
							double y1 = (point1->getY() - _yFocus) * _scale + 0.5 * _heightInPixels;
							double x2 = (point2->getX() - _xFocus) * _scale + 0.5 * _widthInPixels;
							double y2 = (point2->getY() - _yFocus) * _scale + 0.5 * _heightInPixels;

							if ((x1 >= 0 && x1 <= _widthInPixels && y1 >= 0 && y1 <= _heightInPixels) || (x2 >= 0 && x2 <= _widthInPixels && y2 >= 0 && y2 <= _heightInPixels))
							{
								double x = 0.5 * (x1 + x2);
								double y = 0.5 * (y1 + y2);
								double radius1 = parameters.getSegmentRadius1Coeff() * sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
								double radius2 = parameters.getSegmentRadius2Coeff() * size * sizeFactor * _scale;
								double axisDx = x2 - x1;
								double axisDy = y2 - y1;
								RepulsiveCenter *repulsiveCenter = new RepulsiveCenter(&parameters, x, y, axisDx, axisDy, radius1, radius2, parameters.getSegmentRefPotential(), false, false);
								lineItemCopy->addRepulsiveCenter(repulsiveCenter);
							}
						}

                        int lineBuilderCount = itemCopyBuilder->getLineBuilderCount();
                        if (lineBuilderCount != 0)
                        {
                            setTextInfo(lineItemCopy, itemCopyBuilder, sizeFactor, face);

                            int k;
                            for (k = 0; k < lineBuilderCount; ++k)
                            {
                                ItemCopyBuilder *lineBuilder = itemCopyBuilder->getLineBuilder(k);
                                const LineItem *lineItem2 = dynamic_cast<const LineItem *>(lineBuilder->getItem());

                                int j, m = lineItem2->getPointVector(resolutionIndex).size();
                                for (j = 0; j < m; ++j)
                                {
                                    const Point *point = lineItem2->getPointVector(resolutionIndex)[j];
                                    double x = (point->getX() - _xFocus) * _scale + 0.5 * _widthInPixels;
                                    double y = (point->getY() - _yFocus) * _scale + 0.5 * _heightInPixels;
                                    lineItemCopy->addPoint(x, y, j == 0);
                                }
                            }
                        }

						textDisplayer.addItem(lineItemCopy);
					}
					else
					{
						const FilledPolygonItem *filledPolygonItem = dynamic_cast<const FilledPolygonItem *>(item);
						if (filledPolygonItem != 0)
						{
							FilledPolygonItemCopy *filledPolygonItemCopy = new FilledPolygonItemCopy(filledPolygonItem->getElementIdForText(), filledPolygonItem->getImportance());

							int j, m = filledPolygonItem->getPointVector(resolutionIndex).size();
                            for (j = 0; j < m; ++j)
                            {
                                const Point *point = filledPolygonItem->getPointVector(resolutionIndex)[j];
                                double x = (point->getX() - _xFocus) * _scale + 0.5 * _widthInPixels;
                                double y = (point->getY() - _yFocus) * _scale + 0.5 * _heightInPixels;
                                filledPolygonItemCopy->addPoint(x, y);
							}

                            setTextInfo(filledPolygonItemCopy, itemCopyBuilder, sizeFactor, face);
							textDisplayer.addItem(filledPolygonItemCopy);
						}
					}
				}
            }

            delete itemCopyBuilder;
        }

        MapData::unlock();

		FT_Done_FreeType(library);

        bool result = textDisplayer.start();
        if (_svgCreator != 0) _svgCreator->execute();
        return result;
    }

    void RenderRequest::setTextInfo(ItemCopy *itemCopy, ItemCopyBuilder *itemCopyBuilder, double sizeFactor, FT_Face face)
    {
        if (itemCopyBuilder->getTextLook() == 0) return;
        const MapItem *item = itemCopyBuilder->getItem();

        int i, n = item->getTextCount(_languageId);
        for (i = 0; i < n; ++i)
        {
            const ElementName *text = item->getText(_languageId, i);

            if (text != 0)
            {
                std::vector<TextInfoLine *> lineVector;
                int j, m = text->getLineCount();
                for (j = 0; j < m; ++j)
                {
                    std::string line = text->getLine(j);
                    if (!line.empty()) lineVector.push_back(new TextInfoLine(line));
                }

                if (!lineVector.empty())
                {
                    TextInfo *textInfo = new TextInfo(lineVector, floor(itemCopyBuilder->getTextLook()->getSize() * sizeFactor * _scale), itemCopyBuilder->getTextLook(), face);
                    if (textInfo->ok()) itemCopy->addTextInfo(textInfo);
                    else delete textInfo;
                }
            }
        }
    }
}
