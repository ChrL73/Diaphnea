#define _USE_MATH_DEFINES

#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"
#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"
#include "PngImage.h"
#include "PointItemCopy.h"
#include "LineItemCopy.h"
#include "FilledPolygonItemCopy.h"
#include "TextInfo.h"
#include "TextInfoLine.h"
#include "MessageTypeEnum.h"
#include "Interval.h"
#include "SvgCreator.h"
#include "SvgTextInfo.h"
#include "SvgLineInfo.h"

#include <cmath>
#include <chrono>
#include <thread>
#include <limits>
#include <iostream>

namespace map_server
{
	std::mutex TextDisplayer::_mutex;
	int TextDisplayer::_counter = 0;
	std::map<std::string, int *> TextDisplayer::_clientActiveDisplayerMap;
	std::mutex *TextDisplayer::_coutMutexPtr = 0;

	void TextDisplayer::clearClientMap(void)
	{
		_mutex.lock();

		std::map<std::string, int *>::iterator it = _clientActiveDisplayerMap.begin();
		for (; it != _clientActiveDisplayerMap.end(); ++it)
		{
			delete (*it).second;
			(*it).second = 0;
		}

		_mutex.unlock();
	}

    TextDisplayer::TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, const char *requestId,
                                 double width, double height, double xFocus, double yFocus, double scale, bool createPotentialImage, SvgCreator *svgCreator, bool testMode) :
		_parameters(parameters), _socketId(socketId), _requestId(requestId), _width(width), _height(height),
		_xFocus(xFocus), _yFocus(yFocus), _scale(scale), _createPotentialImage(createPotentialImage), _svgCreator(svgCreator), _testMode(testMode)
    {
		_mutex.lock();

        _id = _counter;
        ++_counter;

		std::map<std::string, int *>::iterator it = _clientActiveDisplayerMap.find(socketId);
		if (it == _clientActiveDisplayerMap.end())
		{
			it = _clientActiveDisplayerMap.insert(std::pair<std::string, int *>(socketId, new int)).first;
		}

		_clientActiveDisplayerId = (*it).second;
		*_clientActiveDisplayerId = _id;

		_mutex.unlock();
    }

	TextDisplayer::~TextDisplayer()
	{
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i) delete _itemVector[i];
	}

	bool TextDisplayer::isDisplayerActive(void)
	{
        if (_testMode) return true;

        _mutex.lock();
        bool b = (*_clientActiveDisplayerId == _id);
        _mutex.unlock();
        return b;
	}

    bool TextDisplayer::start(void)
    {
        int visibleTextCount = 0;

        std::multimap<double, ItemCopy *> itemMap;
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i)
		{
            ItemCopy *item = _itemVector[i];
            if (item->getTextInfoCount() > 0) itemMap.insert(std::pair<double, ItemCopy *>(-item->getImportance(), item));
        }

		std::multimap<double, ItemCopy *>::iterator it = itemMap.begin();
		while (it != itemMap.end() && visibleTextCount < _parameters->getMaxVisibleTextCount())
		{
			ItemCopy *item = (*it).second;
			int j, m = item->getTextInfoCount();
			for (j = 0; j < m; ++j)
			{
                if (displayText(item, item->getTextInfo(j)))
                {
                    ++visibleTextCount;
                    break;
                }
            }

            ++it;
		}

        if (_createPotentialImage && isDisplayerActive())
        {
            PngImage image(static_cast<int>(_width), static_cast<int>(_height));
            int j;
            for (i = 0; i < _width && isDisplayerActive(); ++i)
            {
                for (j = 0; j < _height; ++j)
                {
                    Potential p = std::move(getPotential(i, j, 0));

                    double value1 = p.getValue() / _parameters->getPotentialThreshold();
                    if (value1 >= 1.0)
                    {
                        image.setPixel(i, j, 0, 0, 0);
                    }
                    else
                    {
                        if (value1 < 0.0) value1 = 0.0;
                        double r, g, b;
                        hsvToRgb(240.0 * (1.0 - value1), value1, 1.0, r, g, b);
                        image.setPixel(i, j, static_cast<unsigned char>(255.0 * r), static_cast<unsigned char>(255.0 * g), static_cast<unsigned char>(255.0 * b));
                    }
                }
            }

            std::string fileName = "Potential_" + _socketId.substr(2, _socketId.size() - 2) + ".png";
            if (isDisplayerActive()) image.save(fileName);
        }

        return isDisplayerActive();
    }

    bool TextDisplayer::displayText(ItemCopy *item, TextInfo *textInfo)
    {
        if (textInfo == 0) return false;

        PointItemCopy *pointItem = dynamic_cast<PointItemCopy *>(item);
        if (pointItem != 0) return displayPointText(pointItem, textInfo);

        LineItemCopy *lineItem = dynamic_cast<LineItemCopy *>(item);
        if (lineItem != 0) return displayLineText(lineItem, textInfo);

        FilledPolygonItemCopy *filledPolygonItem = dynamic_cast<FilledPolygonItemCopy *>(item);
        if (filledPolygonItem != 0) return displayFilledPolygonText(filledPolygonItem, textInfo);

        return false;
    }

    bool TextDisplayer::displayPointText(PointItemCopy *item, TextInfo *textInfo)
    {
        TextPotential pMin(true);
        double optimalX = 0.0;
        double optimalY = 0.0;

        int i, n = _parameters->getPointTryCount();
        for (i = 0; i < n; ++i)
        {
            double alpha = 2.0 * static_cast<double>(i) * M_PI / static_cast<double>(n);
            double s = sin(alpha);
            if (s > 0.8) s = 0.8;
            else if (s < -0.8) s = -0.8;
            double x = item->getX() + cos(alpha) * (0.5 * textInfo->getWidth() + item->getDiameter());
            double y = item->getY() - s * (0.5 * textInfo->getHeight() + item->getDiameter());
            TextPotential textPotential = std::move(getTextPotential(item, textInfo, x, y, false));

            if (textPotential.compareTo(pMin) < 0.0 && textPotential.getMax() < _parameters->getPotentialThreshold())
            {
                optimalX = x;
                optimalY = y;
                pMin = textPotential;
            }
            if (!isDisplayerActive()) return false;
        }

        if (pMin.getMax() > _parameters->getPotentialThreshold()) return false;

        textInfo->setX(optimalX);
        textInfo->setY(optimalY);
        sendResponse(item, textInfo);

        return true;
    }

    bool TextDisplayer::displayLineText(LineItemCopy *item, TextInfo *textInfo)
    {
        item->setIntersections(_height, _width);

		double optimalY = 0.0;
		double optimalX = 0.0;
		TextPotential pMin(true);

		double yDmin = item->getYMin() + 0.5 * textInfo->getHeight() + 3.0;
		double yDmax = item->getYMax() - 0.5 * textInfo->getHeight() - 3.0;
		double xDmin = item->getXMin() + 0.5 * textInfo->getWidth() + 3.0;
		double xDmax = item->getXMax() - 0.5 * textInfo->getWidth() - 3.0;

        if (yDmax >= yDmin)
        {
            int yCount = _parameters->getLineYTryCount();
            double dy = (yDmax - yDmin) / static_cast<double>(yCount - 1);
            if (dy < 2.0)
            {
                yCount = 1 + static_cast<int>(0.5 * (yDmax - yDmin));

                if (yCount < 2)
                {
                    yCount = 1;
                    yDmin = 0.5 * (yDmax + yDmin);
                    yDmax = yDmin;
                }
                else
                {
                    dy = (yDmax - yDmin) / static_cast<double>(yCount - 1);
                }
            }

            double hD = textInfo->getHeight();
            int hI = static_cast<int>(floor(hD));

            double yD = yDmin;
            int j;
            for (j = 0; j < yCount; ++j)
            {
                int yI = static_cast<int>(floor(yD));

                std::vector<Interval> intervals;
                int i1 = 0;
                intervals.push_back(std::move(Interval(std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max())));
                int y0 = yI - hI / 2;
                int y1 = y0 + hI;
                int y;
                for (y = y0; y <= y1; ++y)
                {
                    int i0 = i1;
                    i1 = intervals.size();

                    std::map<double, double> *intersections = item->getHIntersections(y);

                    if (intersections != 0)
                    {
                        std::map<double, double>::iterator it = intersections->begin();
                        double a = 1.0, dA = 0.0;
                        while (true)
                        {
                            if (a < 1.0) a = 1.0;
                            double b, dB;
                            if (it != intersections->end())
                            {
                                b = (*it).first;
                                dB = (*it).second;
                            }
                            else
                            {
                                b = _width - 1.0;
                                dB = 0.0;
                            }
                            if (b > _width - 1.0) b = _width - 1.0;

                            if (a < _width - 1.0 && b > 1.0)
                            {
                                Interval interval1(a + dA, b - dB);
                                int i;
                                for (i = i0; i < i1; ++i)
                                {
                                    Interval interval2 = std::move(intervals[i].getIntersection(interval1));
                                    if (!interval2.isEmpty() && interval2.getB() - interval2.getA() > textInfo->getWidth())
                                    {
                                        intervals.push_back(std::move(interval2));
                                    }
                                }
                            }

                            a = b;
                            dA = dB;
                            if (it == intersections->end()) break;
                            ++it;
                        }
                    }
                }

                int i, n = intervals.size();
                for (i = i1; i < n; ++i)
                {
                    int k;
                    for (k = 0; k < 2; ++k)
                    {
                        double x;
                        if (k == 0) x = intervals[i].getA() + 0.5 * textInfo->getWidth();
                        else x = intervals[i].getB() - 0.5 * textInfo->getWidth();

                        TextPotential textPotential = std::move(getTextPotential(item, textInfo, x, yD, false));
                        double yc = 2.0 * (yD - yDmin) / (yDmax - yDmin) - 1.0;
                        if (yc < 0) yc = -yc;
                        textPotential.add(_parameters->getCenteringPotential() * yc);

                        if (textPotential.compareTo(pMin) < 0.0 && textPotential.getMax() < _parameters->getPotentialThreshold())
                        {
                            optimalX = x;
                            optimalY = yD;
                            pMin = textPotential;
                        }
                    }
                }

                yD += dy;
            }
		}

		if (xDmax >= xDmin)
        {
            int xCount = _parameters->getLineXTryCount();
            double dx = (xDmax - xDmin) / static_cast<double>(xCount - 1);
            if (dx < 2.0)
            {
                xCount = 1 + static_cast<int>(0.5 * (xDmax - xDmin));

                if (xCount < 2)
                {
                    xCount = 1;
                    xDmin = 0.5 * (xDmax + xDmin);
                    xDmax = xDmin;
                }
                else
                {
                    dx = (xDmax - xDmin) / static_cast<double>(xCount - 1);
                }
            }

            double wD = textInfo->getWidth();
            int wI = static_cast<int>(floor(wD));

            double xD = xDmin;
            int j;
            for (j = 0; j < xCount; ++j)
            {
                int xI = static_cast<int>(floor(xD));

                std::vector<Interval> intervals;
                int i1 = 0;
                intervals.push_back(std::move(Interval(std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max())));
                int x0 = xI - wI / 2;
                int x1 = x0 + wI;
                int x;
                for (x = x0; x <= x1; ++x)
                {
                    int i0 = i1;
                    i1 = intervals.size();

                    std::map<double, double> *intersections = item->getVIntersections(x);

                    if (intersections != 0)
                    {
                        std::map<double, double>::iterator it = intersections->begin();
                        double a = 1.0;
                        double dA = 0.0;
                        while (true)
                        {
                            if (a < 1.0) a = 1.0;
                            double b, dB;
                            if (it != intersections->end())
                            {
                                b = (*it).first;
                                dB = (*it).second;
                            }
                            else
                            {
                                b = _height - 1.0;
                                dB = 0.0;
                            }
                            if (b > _height - 1.0) b = _height - 1.0;

                            if (a < _height - 1.0 && b > 1.0)
                            {
                                Interval interval1(a + dA, b - dB);
                                int i;
                                for (i = i0; i < i1; ++i)
                                {
                                    Interval interval2 = std::move(intervals[i].getIntersection(interval1));
                                    if (!interval2.isEmpty() && interval2.getB() - interval2.getA() > textInfo->getHeight())
                                    {
                                        intervals.push_back(std::move(interval2));
                                    }
                                }
                            }

                            a = b;
                            dA = dB;
                            if (it == intersections->end()) break;
                            ++it;
                        }
                    }
                }

                int i, n = intervals.size();
                for (i = i1; i < n; ++i)
                {
                    int k;
                    for (k = 0; k < 2; ++k)
                    {
                        double y;
                        if (k == 0) y = intervals[i].getA() + 0.5 * textInfo->getHeight();
                        else y = intervals[i].getB() - 0.5 * textInfo->getHeight();

                        TextPotential textPotential = std::move(getTextPotential(item, textInfo, xD, y, false));
                        double xc = 2.0 * (xD - xDmin) / (xDmax - xDmin) - 1.0;
                        if (xc < 0) xc = -xc;
                        textPotential.add(_parameters->getCenteringPotential() * xc);

                        if (textPotential.compareTo(pMin) < 0.0 && textPotential.getMax() < _parameters->getPotentialThreshold())
                        {
                            optimalX = xD;
                            optimalY = y;
                            pMin = textPotential;
                        }
                    }
                }

                xD += dx;
            }
		}

		if (pMin.getMax() > _parameters->getPotentialThreshold()) return false;

		textInfo->setX(optimalX);
		textInfo->setY(optimalY);
		sendResponse(item, textInfo);

        return false;
    }

    bool TextDisplayer::displayFilledPolygonText(FilledPolygonItemCopy *item, TextInfo *textInfo)
    {
        item->setIntersections(_height, _width);

        double optimalYD = 0.0;
        double optimalX = 0.0;
        TextPotential pMin(true);

        double yDmin = item->getYMin() + 0.5 * textInfo->getHeight() + 3.0;
        double yDmax = item->getYMax() - 0.5 * textInfo->getHeight() - 3.0;
        if (yDmax < yDmin) return false;

        int yCount = _parameters->getPolygonYTryCount();;
        double dy = (yDmax - yDmin) / static_cast<double>(yCount - 1);
        if (dy < 2.0)
        {
            yCount = 1 + static_cast<int>(0.5 * (yDmax - yDmin));

            if (yCount < 2)
            {
                yCount = 1;
                yDmin = 0.5 * (yDmax + yDmin);
                yDmax = yDmin;
            }
            else
            {
                dy = (yDmax - yDmin) / static_cast<double>(yCount - 1);
            }
        }

        double hD = textInfo->getHeight();
        int hI = static_cast<int>(floor(hD));

        double yD = yDmin;
        int j;
        for (j = 0; j < yCount; ++j)
        {
            int yI = static_cast<int>(floor(yD));

            std::vector<Interval> intervals;
            int i1 = 0;
            intervals.push_back(std::move(Interval(std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max())));
            int y0 = yI - hI / 2;
            int y1 = y0 + hI;
            int y;
            for (y = y0; y <= y1; ++y)
            {
                int i0 = i1;
                i1 = intervals.size();

                std::set<double> *intersections = item->getIntersections(y);

                if (intersections != 0)
                {
                    std::set<double>::iterator it = intersections->begin();
                    for (; it != intersections->end(); ++it)
                    {
                        double a = *it;
                        if (a < 1.0) a = 1.0;
                        ++it;
                        double b = *it;
                        if (b > _width - 1.0) b = _width - 1.0;

                        if (a < _width - 1.0 && b > 1.0)
                        {
                            Interval interval1(a, b);
                            int i;
                            for (i = i0; i < i1; ++i)
                            {
                                Interval interval2 = std::move(intervals[i].getIntersection(interval1));
                                if (!interval2.isEmpty() && interval2.getB() - interval2.getA() > textInfo->getWidth())
                                {
                                    intervals.push_back(std::move(interval2));
                                }
                            }
                        }
                    }
                }
            }

            int i, n = intervals.size();
            for (i = i1; i < n; ++i)
            {
                double x0 = intervals[i].getA() + 0.5 * textInfo->getWidth();
                double x1 = intervals[i].getB() - 0.5 * textInfo->getWidth();

                int xCount = _parameters->getPolygonXTryCount();
                double dx = (x1 - x0) / static_cast<double>(xCount - 1);
                if (dx < 2.0)
                {
                    xCount = 1 + static_cast<int>(0.5 * (x1 - x0));

                    if (xCount < 2)
                    {
                        xCount = 1;
                        x0 = 0.5 * (x0 + x1);
                        x1 = x0;
                    }
                    else
                    {
                        dx = (x1 - x0) / static_cast<double>(xCount - 1);
                    }
                }

                double x = x0;
                int k;
                for (k = 0; k < xCount; ++k)
                {
                    TextPotential textPotential = std::move(getTextPotential(item, textInfo, x, yD, true));

                    double xc = 2.0 * (x - x0) / (x1 - x0) - 1.0;
                    if (xc < 0) xc = -xc;
                    double yc = 2.0 * (yD - yDmin) / (yDmax - yDmin) - 1.0;
                    if (yc < 0) yc = -yc;
                    textPotential.add(_parameters->getCenteringPotential() * (xc + yc));

                    if (textPotential.compareTo(pMin) < 0.0 && textPotential.getMax() < _parameters->getPotentialThreshold())
                    {
                        optimalX = x;
                        optimalYD = yD;
                        pMin = textPotential;
                    }

                    if (!isDisplayerActive()) return false;
                    x += dx;
                }
            }

            yD += dy;
        }

        if (pMin.getMax() > _parameters->getPotentialThreshold()) return false;

        textInfo->setX(optimalX);
        textInfo->setY(optimalYD);
        sendResponse(item, textInfo);

        return true;
    }

    TextPotential TextDisplayer::getTextPotential(ItemCopy *item, TextInfo *textInfo, double x, double y, bool selfRepulsion)
    {
        TextPotential textPotential;

        double centereCountXD = ceil(_parameters->getComputationDensityFactor() * textInfo->getWidth());
        if (centereCountXD < 1.0) centereCountXD = 1.0;
        centereCountXD = centereCountXD * 2 + 1;
        double dx = textInfo->getWidth() / (centereCountXD - 1.0);
        int centerCountX = static_cast<int>(floor(centereCountXD + 0.1));

        double centereCountYD = ceil(_parameters->getComputationDensityFactor() * textInfo->getHeight());
        if (centereCountYD < 1.0) centereCountYD = 1.0;
        centereCountYD = centereCountYD * 2 + 1;
        double dy = textInfo->getHeight() / (centereCountYD - 1.0);
        int centerCountY = static_cast<int>(floor(centereCountYD + 0.1));

        x -= 0.5 * textInfo->getWidth();
        y -= 0.5 * textInfo->getHeight();
        int i, j;
        for (i = 0; i < centerCountX; ++i)
        {
            for (j = 0; j < centerCountY; ++j)
            {
                if ((i + j) % 2 == 0)
                {
                    Potential p = std::move(getPotential(x + static_cast<double>(i) * dx, y + static_cast<double>(j) * dy, selfRepulsion ? 0 : item));
                    textPotential.add(p.getValue());
                    if (textPotential.getMax() > _parameters->getPotentialThreshold()) return std::move(textPotential);
                }
            }
        }

        return std::move(textPotential);
    }

    void TextDisplayer::sendResponse(ItemCopy *item, TextInfo *textInfo)
    {
        double xMin = textInfo->getX()  - 0.5 * textInfo->getWidth();
        double xMax = textInfo->getX()  + 0.5 * textInfo->getWidth();
        double yMin = textInfo->getY()  - 0.5 * textInfo->getHeight();
        double yMax = textInfo->getY()  + 0.5 * textInfo->getHeight();

        xMin = _xFocus + (xMin - 0.5 * _width) / _scale;
        xMax = _xFocus + (xMax - 0.5 * _width) / _scale;
        yMin = _yFocus + (yMin - 0.5 * _height) / _scale;
        yMax = _yFocus + (yMax - 0.5 * _height) / _scale;

        SvgTextInfo *svgTextInfo = 0;
        if (_svgCreator != 0) svgTextInfo = new SvgTextInfo(textInfo);

        _coutMutexPtr->lock();
        std::cout << _socketId << " " << _requestId << " " << map_server::TEXT
            << " {\"t\":[";

        int i, n = textInfo->getLineVector().size();
        for (i = 0; i < n; ++i)
        {
            const TextInfoLine *line = textInfo->getLineVector()[i];
            double x = textInfo->getX() - 0.5 * textInfo->getWidth() - line->getXOffset();
            double y = textInfo->getY() + 0.5 * textInfo->getHeight() + line->getYOffset();
            x = _xFocus + (x - 0.5 * _width) / _scale;
            y = _yFocus + (y - 0.5 * _height) / _scale;

            std::cout << "[\"" << line->getText() << "\","
                << x << "," << y << "]";
            if (i != n - 1) std::cout << ",";

            if (_svgCreator != 0) svgTextInfo->addLine(new SvgLineInfo(line->getText(), x, y));
        }

        std::cout  << "],\"e\":\"" << item->getElementId()
            << "\",\"x1\":" << xMin
            << ",\"x2\":" << xMax
            << ",\"y1\":" << yMin
            << ",\"y2\":" << yMax
            << ",\"s\":" << textInfo->getFontSize()
            << ",\"z\":" << textInfo->getZIndex()
            << ",\"a\":" << textInfo->getAlpha()
            << ",\"r\":" << textInfo->getRed()
            << ",\"g\":" << textInfo->getGreen()
            << ",\"b\":" << textInfo->getBlue()
            << "}" << std::endl;
        _coutMutexPtr->unlock();

        if (_svgCreator != 0) _svgCreator->addInfo(textInfo->getZIndex(), svgTextInfo);

        RepulsiveCenter *center = new RepulsiveCenter(_parameters, textInfo->getX(), textInfo->getY(), 1.0, 0.0,
            _parameters->getTextRadiusCoeff() * textInfo->getWidth(), _parameters->getTextRadiusCoeff() * textInfo->getHeight(), _parameters->getTextRefPotential(), true, true);
        item->addRepulsiveCenter(center);
    }

    Potential TextDisplayer::getPotential(double x, double y, ItemCopy *selfItem)
    {
        Potential potential;

        const double u0 = _parameters->getEdgeRefPotential();
        const double epsilon = _parameters->getEdgeRangeRatio();
        double a;

        a = x / (epsilon * _width) - 1;
        if (a < 0) potential.add(u0 * a * a);

        a = y / (epsilon * _height) - 1;
        if (a < 0) potential.add(u0 * a * a);

        a = (x - _width) / (epsilon * _width) + 1;
        if (a > 0) potential.add(u0 * a * a);

        a = (y - _height) / (epsilon * _height) + 1;
        if (a > 0) potential.add(u0 * a * a);

        int i, n = _itemVector.size();
        for (i = 0; i < n; ++i)
        {
            ItemCopy *item = _itemVector[i];
            if (selfItem == 0 || item != selfItem)
            {
                Potential elementaryPotential = std::move(getElementaryPotential(item, x, y));
                potential += elementaryPotential;
                if (potential.getValue() > _parameters->getPotentialThreshold()) return std::move(potential);
            }
        }

        return std::move(potential);
    }

    Potential TextDisplayer::getElementaryPotential(ItemCopy *item, double x, double y)
    {
        Potential potential;

        int i, n = item->getRepulsiveCenterVector().size();
        for (i = 0; i < n; ++i)
        {
            const RepulsiveCenter *center = item->getRepulsiveCenterVector()[i];
            double dx = center->getX() - x;
            double dy = center->getY() - y;

            double R1 = center->getV11() * dx + center->getV12() * dy;
            double R2 = center->getV21() * dx + center->getV22() * dy;

            if (center->getExcluding())
            {
                int R;
                if (center->getRectangle())
                {
                    R1 *= R1;
                    R2 *= R2;
                    R = static_cast<int>(R1 > R2 ? R1 : R2);
                }
                else
                {
                    R = static_cast<int>(R1 * R1 + R2 * R2);
                }

                if (R < _parameters->getPotentialTableSize())
                {
                    potential.add(center->getU0() * _parameters->getExcludingPotential(R));

                    if (potential.getValue() > _parameters->getPotentialThreshold()) break;
                }
            }
            else
            {
                double p = 0.0;

                int R = static_cast<int>(R1 * R1);
                if (R < _parameters->getPotentialTableSize())
                {
                    p = _parameters->getNotExcludingPotential1(R);

                    R = static_cast<int>(R2 * R2);
                    if (R < _parameters->getPotentialTableSize())
                    {
                        p *= _parameters->getNotExcludingPotential2(R);
                    }
                    else
                    {
                        p = 0.0;
                    }
                }

                potential.add(center->getU0() * p);

                if (potential.getValue() > _parameters->getPotentialThreshold()) break;
            }
        }

        return std::move(potential);
    }

    void TextDisplayer::hsvToRgb(double h, double s, double v, double& r, double& g, double& b)
    {
        double hh, p, q, t, ff;
        long i;

        if (s <= 0.0)
        {
            r = v;
            g = v;
            b = v;
            return;
        }

        hh = h;
        if (hh >= 360.0) hh = 0.0;
        hh /= 60.0;
        i = (long)hh;
        ff = hh - i;
        p = v * (1.0 - s);
        q = v * (1.0 - (s * ff));
        t = v * (1.0 - (s * (1.0 - ff)));

        switch(i)
        {
        case 0:
            r = v;
            g = t;
            b = p;
            break;

        case 1:
            r = q;
            g = v;
            b = p;
            break;

        case 2:
            r = p;
            g = v;
            b = t;
            break;

        case 3:
            r = p;
            g = q;
            b = v;
            break;

        case 4:
            r = t;
            g = p;
            b = v;
            break;

        default:
            r = v;
            g = p;
            b = q;
            break;
        }
    }
}
