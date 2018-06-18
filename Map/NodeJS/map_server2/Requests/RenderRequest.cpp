#include "RenderRequest.h"
#include "MessageTypeEnum.h"
#include "PointElement.h"
#include "LineElement.h"
#include "PolygonElement.h"
#include "MultipointItem.h"
#include "CommonData.h"
#include "PointItem.h"
#include "FilledPolygonItem.h"
#include "LineItem.h"
#include "TextDisplayer.h"
#include "RepulsiveCenter.h"
#include "PointList.h"
#include "NameTranslation.h"
#include "TextInfo.h"
#include "TextInfoLine.h"
#include "TextDisplayerParameters.h"
#include "ElementName.h"
#include "SvgCreator.h"
#include "SvgItemInfo.h"
#include "ErrorEnum.h"
#include "SvgCustomColor.h"

#include "ft2build.h"
#include FT_FREETYPE_H

#include <cstring>
#include <sstream>
#include <limits>
#include <cmath>
#include <iostream>

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

        std::map<int, PointItem *> pointItems;
        std::map<int, LineItem *> lineItems;
        std::map<int, FilledPolygonItem *> filledPolygonItems;
        std::vector<MapItem *> itemVector;
        std::set<std::string> coveredElementSet;

        int languageIndex = _commonData->getLanguageIndex(_languageId);

        unsigned int i, n = _elementIds.size();
        for (i = 0; i < n; ++i)
        {
            std::string elementId = _elementIds[i];

            _commonData->lock();
            ElementTypeEnum elementType = _commonData->getElementType(elementId);

            if (elementType == map_server::POINT)
            {
                const PointElement *pointElement = _commonData->getLastElementAsPoint();
                _commonData->unlock();

                std::map<int, PointItem *>::iterator pointItemIt = pointItems.find(pointElement->getItemId());
                if (pointItemIt == pointItems.end())
                {
                    const NameTranslation *name = 0;
                    if (languageIndex != -1) name = pointElement->getTranslation(languageIndex);

                    PointItem *item = new PointItem(pointElement, name, pointElement->getLook(_lookIndex));
                    pointItems.insert(std::pair<int, PointItem *>(pointElement->getItemId(), item));
                    itemVector.push_back(item);
                }
            }
            else if (elementType == map_server::LINE)
            {
                const LineElement *lineElement = _commonData->getLastElementAsLine();
                _commonData->unlock();

                int j, m = lineElement->getItemCount();
                for (j = 0; j < m; ++j)
                {
                    const MultipointItem *multipointItem = lineElement->getItem(j);

                    std::map<int, LineItem *>::iterator lineItemIt = lineItems.find(multipointItem->getItemId());
                    if (lineItemIt == lineItems.end())
                    {
                        const NameTranslation *name = 0;
                        if (languageIndex != -1) name = lineElement->getTranslation(languageIndex);

                        LineItem *lineItem = new LineItem(lineElement, name, multipointItem, lineElement->getLook(_lookIndex));
                        lineItems.insert(std::pair<int, LineItem *>(multipointItem->getItemId(), lineItem));
                        itemVector.push_back(lineItem);
                    }
                }
            }
            else if (elementType == map_server::POLYGON)
            {
                const PolygonElement *polygonElement = _commonData->getLastElementAsPolygon();
                _commonData->unlock();

                const MultipointItem *contourItem = polygonElement->getContour();

                std::map<int, FilledPolygonItem *>::iterator filledPolygonItemIt = filledPolygonItems.find(contourItem->getItemId());
                if (filledPolygonItemIt == filledPolygonItems.end())
                {
                    const NameTranslation *name = 0;
                    if (languageIndex != -1) name = polygonElement->getTranslation(languageIndex);

                    FilledPolygonItem *filledPolygonItem = new FilledPolygonItem(polygonElement, name, contourItem, polygonElement->getLook(_lookIndex));
                    filledPolygonItems.insert(std::pair<int, FilledPolygonItem *>(contourItem->getItemId(), filledPolygonItem));
                    itemVector.push_back(filledPolygonItem);
                }

                int j, m = polygonElement->getItemCount();
                for (j = 0; j < m; ++j)
                {
                    const MultipointItem *item = polygonElement->getItem(j);

                    std::map<int, LineItem *>::iterator lineItemIt = lineItems.find(item->getItemId());
                    if (lineItemIt == lineItems.end())
                    {
                        LineItem *lineItem = new LineItem(item, polygonElement->getLook(_lookIndex));
                        lineItems.insert(std::pair<int, LineItem *>(item->getItemId(), lineItem));
                        itemVector.push_back(lineItem);
                    }
                    else
                    {
                        (*lineItemIt).second->updateLook(polygonElement->getLook(_lookIndex));
                    }
                }

                m = polygonElement->getCoveredElementCount();
                for (j = 0; j < m; ++j) coveredElementSet.insert(polygonElement->getCoveredElement(j));
            }
            else if (elementId == "#test")
            {
                _commonData->unlock();
                _testMode = true;
            }
            // Disable potential image creation, because it's only a developement/debug/test feature.
            // Moreover, this  feature requires a significant calculation time that could overload the server if used in production.
            /*else if (elementId == "#img")
            {
                _commonData->unlock();
                _createPotentialImage = true;
            }*/
            else
            {
                _commonData->unlock();
            }
        }

        n = itemVector.size();

        if (n != 0)
        {
            double xMin, xMax, yMin, yMax;

            if (!_focusSetByClient)
            {
                xMin = std::numeric_limits<double>::max();
                xMax = std::numeric_limits<double>::lowest();
                yMin = xMin;
                yMax = xMax;
                double xMin0 = xMin;
                double xMax0 = xMax;
                double yMin0 = xMin;
                double yMax0 = xMax;
                bool minMaxSet = false;

                for (i = 0; i < n; ++i)
                {
                    MapItem *item = itemVector[i];

                    int framingLevel = 0;
                    if (item->getFramingLevel() != -1 && _framingExceptions.find(item->getElementId()) == _framingExceptions.end())
                    {
                        framingLevel = item->getFramingLevel();
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

                if (geoSize < zoomMinDistance) geoSize = zoomMinDistance;
                if (geoSize > zoomMaxDistance) geoSize = zoomMaxDistance;

                _scale = sqrt(_widthInPixels * _widthInPixels + _heightInPixels * _heightInPixels) / geoSize;
            }

            double wantedLength = resolutionThreshold / _scale;
            int resolutionIndex;
            for (resolutionIndex = sampleLengthCount - 1; resolutionIndex > 0; --resolutionIndex)
            {
                if (wantedLength > sampleLengths[resolutionIndex]) break;
            }

            const double r = 0.6;
            double dx = r * _widthInPixels / _scale;
            xMin = _xFocus - dx;
            xMax = _xFocus + dx;
            double dy = r * _heightInPixels / _scale;
            yMin = _yFocus - dy;
            yMax = _yFocus + dy;

            std::vector<MapItem *> itemVector2;

            for (i = 0; i < n; ++i)
            {
                MapItem *item = itemVector[i];
                if (item->getXMax() >= xMin && item->getXMin() <= xMax && item->getYMax() >= yMin && item->getYMin() <= yMax)
                {
                    itemVector2.push_back(item);
                }
            }

            dx = 0.5 * _widthInPixels / _scale;
            xMin = _xFocus - dx;
            xMax = _xFocus + dx;
            dy = 0.5 * _heightInPixels / _scale;
            yMin = _yFocus - dy;
            yMax = _yFocus + dy;

            double sizeFactor = sizeParameter1 / (sizeParameter2 + _scale);

            std::stringstream response;

            if (createSvg) _svgCreator = new SvgCreator(_widthInPixels, _heightInPixels, _scale, sizeFactor, _xFocus, _yFocus, _socketId, _requestId, &_customColorMap);
            else response << _socketId << " " << _requestId << " " << map_server::RENDER << " {\"items\":[";

            std::map<std::string, std::vector<LineItem *> > lineItemAssociationMap;

            n = itemVector2.size();
            for (i = 0; i < n; ++i)
            {
                MapItem *item = itemVector2[i];

                if (_svgCreator == 0)
                {
                    if (i != 0) response << ",";
                    response << "[" << item->getItemId() << "," << item->getElementLookId();
                    if (item->hasResolution()) response << "," << resolutionIndex;
                    response << "]";
                }
                else
                {
                    SvgItemInfo *svgItemInfo = new SvgItemInfo(item, resolutionIndex);
                    _svgCreator->addInfo(item->getZIndex(), svgItemInfo);
                }

                if (languageIndex != -1 && coveredElementSet.find(item->getElementId()) == coveredElementSet.end() &&
                    item->getXMax() >= xMin && item->getXMin() <= xMax && item->getYMax() >= yMin && item->getYMin() <= yMax)
                {
                    _itemVector3.push_back(item);

                    LineItem *lineItem = dynamic_cast<LineItem *>(item);
                    if (lineItem != 0)
                    {
                        std::string elementId = lineItem->getElementId();
                        if (!elementId.empty())
                        {
                            std::map<std::string, std::vector<LineItem *> >::iterator elementIt = lineItemAssociationMap.find(elementId);
                            if (elementIt == lineItemAssociationMap.end())
                            {
                                elementIt = lineItemAssociationMap.insert(std::pair<std::string, std::vector<LineItem *> >(elementId, std::vector<LineItem *>())).first;
                            }

                            (*elementIt).second.push_back(lineItem);
                        }
                    }
                }
                else
                {
                    if (_svgCreator == 0 && !item->getElementId().empty())
                    {
                        std::stringstream delResponse;
                        delResponse << _socketId << " " << _requestId << " " << map_server::REMOVE_TEXT
                            << " {\"e\":\"" << item->getElementId() << "\"}";

                        _coutMutexPtr->lock();
                        std::cout << delResponse.str() << std::endl;
                        _coutMutexPtr->unlock();
                    }
                }
            }

            if (_svgCreator == 0)
            {
                response << "],\"xFocus\":" << _xFocus << ",\"yFocus\":" << _yFocus
                         << ",\"scale\":" << _scale << "}";
            }

            std::map<std::string, std::vector<LineItem *> >::iterator elementIt = lineItemAssociationMap.begin();
            for (; elementIt != lineItemAssociationMap.end(); ++elementIt)
            {
                (*elementIt).second[0]->setAssociatedLineVector((*elementIt).second);
            }

            if (_svgCreator == 0)
            {
                _coutMutexPtr->lock();
                std::cout << response.str() << std::endl;
                _coutMutexPtr->unlock();
            }

            if (languageIndex != -1)
            {
                displayText(resolutionIndex);
            }
            else
            {
                _coutMutexPtr->lock();
                std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::UNKNOWN_ID
                    << ",\"message\":\"Unknown language id ('" << _languageId << "') in RENDER request\"}" << std::endl;
                _coutMutexPtr->unlock();
            }
        }
        else if (itemVector.empty())
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

        n = itemVector.size();
        for (i = 0; i < n; ++i) delete itemVector[i];
    }

    void RenderRequest::displayText(int resolutionIndex)
    {
        const std::string fontFile = "arial.ttf";

        FT_Library  library;
        int error = FT_Init_FreeType(&library);
        if (error)
        {
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::FREE_TYPE_INIT_FAILED
                << ",\"message\":\"FreeType library initialization failed\"}" << std::endl;
            _coutMutexPtr->unlock();
            return;
        }

        FT_Face face;
        error = FT_New_Face(library, fontFile.c_str(), 0, &face);
        if (error)
        {
            FT_Done_FreeType(library);
            _coutMutexPtr->lock();
            std::cout << _socketId << " " << _requestId << " " << map_server::ERROR_ << " {\"error\":" << map_server::FONT_NOT_FOUND
                << ",\"message\":\"Failed to load arial.ttf font\"}" << std::endl;
            _coutMutexPtr->unlock();
            return;
        }

        TextDisplayerParameters parameters;
        TextDisplayer textDisplayer(&parameters, _socketId, _requestId, _widthInPixels, _heightInPixels, _xFocus, _yFocus, _scale, _createPotentialImage, _svgCreator, _testMode);

        double sizeFactor = sizeParameter1 / (sizeParameter2 + _scale);

        int i, n = _itemVector3.size();
        for (i = 0; i < n; ++i)
        {
            MapItem *item = _itemVector3[i];

            PointItem *pointItem = dynamic_cast<PointItem *>(item);
            if (pointItem != 0)
            {
                double diameter = pointItem->getPointSize() * sizeFactor * _scale;
                double x = (pointItem->getXMin() - _xFocus) * _scale + 0.5 * _widthInPixels;
                double y = (pointItem->getYMin() - _yFocus) * _scale + 0.5 * _heightInPixels;

                pointItem->setDiameter(diameter);
                pointItem->setX(x);
                pointItem->setY(y);

                double radius = parameters.getPointRadiusCoeff() * diameter;
                RepulsiveCenter *repulsiveCenter = new RepulsiveCenter(&parameters, x, y, 1.0, 0.0, radius, radius, parameters.getPointRefPotential(), true, false);
                pointItem->addRepulsiveCenter(repulsiveCenter);

                pointItem->setFontSize(floor(pointItem->getTextSize() * sizeFactor * _scale));
                setTextInfo(pointItem, face);

                textDisplayer.addItem(pointItem);
            }
            else
            {
                LineItem *lineItem = dynamic_cast<LineItem *>(item);
                if (lineItem != 0)
                {
                    const PointList *pointList = lineItem->getPointList(resolutionIndex);
                    int j, m = pointList->getPointCount();
                    for (j = 0; j < m - 1; ++j)
                    {
                        const double *point1 = pointList->getPoint(j);
                        const double *point2 = pointList->getPoint(j + 1);
                        double x1 = (point1[4] - _xFocus) * _scale + 0.5 * _widthInPixels;
                        double y1 = (point1[5] - _yFocus) * _scale + 0.5 * _heightInPixels;
                        double x2 = (point2[4] - _xFocus) * _scale + 0.5 * _widthInPixels;
                        double y2 = (point2[5] - _yFocus) * _scale + 0.5 * _heightInPixels;

                        if ((x1 >= 0 && x1 <= _widthInPixels && y1 >= 0 && y1 <= _heightInPixels) || (x2 >= 0 && x2 <= _widthInPixels && y2 >= 0 && y2 <= _heightInPixels))
                        {
                            double x = 0.5 * (x1 + x2);
                            double y = 0.5 * (y1 + y2);
                            double radius1 = parameters.getSegmentRadius1Coeff() * sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
                            double radius2 = parameters.getSegmentRadius2Coeff() * lineItem->getLineSize() * sizeFactor * _scale;
                            double axisDx = x2 - x1;
                            double axisDy = y2 - y1;
                            RepulsiveCenter *repulsiveCenter = new RepulsiveCenter(&parameters, x, y, axisDx, axisDy, radius1, radius2, parameters.getSegmentRefPotential(), false, false);
                            lineItem->addRepulsiveCenter(repulsiveCenter);
                        }
                    }

                    int associatedLineCount = lineItem->getAssociatedLineCount();
                    if (associatedLineCount != 0)
                    {
                        lineItem->setFontSize(floor(lineItem->getTextSize() * sizeFactor * _scale));
                        setTextInfo(lineItem, face);

                        int k;
                        for (k = 0; k < associatedLineCount; ++k)
                        {
                            LineItem *associatedLine = lineItem->getAssociatedLine(k);
                            const PointList *pointList2 = associatedLine->getPointList(resolutionIndex);

                            int j, m = pointList2->getPointCount();
                            for (j = 0; j < m; ++j)
                            {
                                const double *point = pointList2->getPoint(j);
                                double x = (point[4] - _xFocus) * _scale + 0.5 * _widthInPixels;
                                double y = (point[5] - _yFocus) * _scale + 0.5 * _heightInPixels;
                                lineItem->addPoint(x, y, j == 0);
                            }
                        }
                    }

                    textDisplayer.addItem(lineItem);
                }
                else
                {
                    FilledPolygonItem *filledPolygonItem = dynamic_cast<FilledPolygonItem *>(item);
                    if (filledPolygonItem != 0)
                    {
                        const PointList *pointList = filledPolygonItem->getPointList(resolutionIndex);
                        int j, m = pointList->getPointCount();
                        for (j = 0; j < m; ++j)
                        {
                            const double *point = pointList->getPoint(j);
                            double x = (point[4] - _xFocus) * _scale + 0.5 * _widthInPixels;
                            double y = (point[5] - _yFocus) * _scale + 0.5 * _heightInPixels;
                            filledPolygonItem->addPoint(x, y);
                        }

                        filledPolygonItem->setFontSize(floor(filledPolygonItem->getTextSize() * sizeFactor * _scale));
                        setTextInfo(filledPolygonItem, face);
                        textDisplayer.addItem(filledPolygonItem);
                    }
                }
            }
        }

        FT_Done_FreeType(library);

        textDisplayer.start();
        if (_svgCreator != 0) _svgCreator->execute();
    }

    void RenderRequest::setTextInfo(MapItem *item, FT_Face face)
    {
        const NameTranslation *name = item->getName();
        if (name == 0) return;

        int i, n = name->getNameCount();
        for (i = 0; i < n; ++i)
        {
            const ElementName *text = name->getElementName(i);

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
                    TextInfo *textInfo = new TextInfo(lineVector, face, item->getFontSize());
                    if (textInfo->ok()) item->addTextInfo(textInfo);
                    else delete textInfo;
                }
            }
        }
    }
}
