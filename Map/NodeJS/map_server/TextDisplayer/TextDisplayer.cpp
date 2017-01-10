#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"
#include "ClientInfo.h"
#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"
#include "PngImage.h"
#include "PointItemCopy.h"
#include "LineItemCopy.h"
#include "FilledPolygonItemCopy.h"
#include "TextInfo.h"
#include "MessageTypeEnum.h"

#include <chrono>
#include <thread>
#include <limits>
#include <cmath>
#include <iostream>

namespace map_server
{
	std::mutex TextDisplayer::_mutex;
	std::map<std::string, ClientInfo *> TextDisplayer::_clientMap;
	std::mutex *TextDisplayer::_coutMutexPtr = 0;

	void TextDisplayer::clearClientMap(void)
	{
		_mutex.lock();

		std::map<std::string, ClientInfo *>::iterator it = _clientMap.begin();
		for (; it != _clientMap.end(); ++it)
		{
			delete (*it).second;
			(*it).second = 0;
		}

		_mutex.unlock();
	}

    TextDisplayer::TextDisplayer(const TextDisplayerParameters *parameters, const char *socketId, const char *requestId, double width, double height, bool createPotentialImage) :
		_parameters(parameters), _socketId(socketId), _requestId(requestId), _width(width), _height(height), _createPotentialImage(createPotentialImage)
    {
		_mutex.lock();

		std::map<std::string, ClientInfo *>::iterator it = _clientMap.find(socketId);
		if (it == _clientMap.end())
		{
			it = _clientMap.insert(std::pair<std::string, ClientInfo *>(socketId, new ClientInfo())).first;
		}

		_clientInfo = (*it).second;
		_clientInfo->setStopRequested(true);

		_mutex.unlock();
    }

	TextDisplayer::~TextDisplayer()
	{
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i) delete _itemVector[i];
	}

	bool TextDisplayer::isStopRequested(void)
	{
        _mutex.lock();
        bool b = _clientInfo->isStopRequested();
        _mutex.unlock();
        return b;
	}

    void TextDisplayer::start(void)
    {
        _mutex.lock();

        while(_clientInfo->getThreadCount() != 0) std::this_thread::sleep_for(std::chrono::milliseconds(10));
        _clientInfo->setStopRequested(false);
        _clientInfo->incrementThreadCount();

        _mutex.unlock();

		int visibleTextCount = 0;

		int i, n = _itemVector.size();
		for (i = 0; i < n && visibleTextCount < _parameters->getMaxVisibleTextCount(); ++i)
		{
			ItemCopy *item = _itemVector[i];
			if (displayText(item, item->getTextInfo1())) ++visibleTextCount;
			else if (displayText(item, item->getTextInfo2())) ++visibleTextCount;
		}

        if (_createPotentialImage && !isStopRequested())
        {
            PngImage image(static_cast<int>(_width), static_cast<int>(_height));
            int j;
            for (i = 0; i < _width && !isStopRequested(); ++i)
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
                        image.setPixel(i, j, 255 * r, 255 * g, 255 * b);
                    }
                }
            }

            if (!isStopRequested()) image.save("potential.png");
        }

        _mutex.lock();
        _clientInfo->decrementThreadCount();
        _mutex.unlock();
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
        Potential pMin(std::numeric_limits<double>::max());
        double alphaMin = 0.0;
        int i, n = 12;
        for (i = 0; i < n; ++i)
        {
            double alpha = 2.0 * static_cast<double>(i) * M_PI / static_cast<double>(n);
            /*Potential potential = getPotential(pointItem, item.CurrentWidth, item.CurrentHeight, Math.Cos(alpha), Math.Sin(alpha), false);
            if (potential.compareTo(pMin) < 0.0 && potential.isAcceptable(_softThreshold))
            {
                alphaMin = alpha;
                pMin = potential;
            }
            if (_stopRequested) return false;*/
        }

        //if (!pMin.isAcceptable(_softThreshold)) return false;

        double s = sin(alphaMin);
        if (s > 0.8) s = 0.8;
        else if (s < -0.8) s = -0.8;
        textInfo->setX(item->getX() + cos(alphaMin) * (0.5 * textInfo->getWidth() + 0.5 * item->getDiameter()) - 0.5 * textInfo->getWidth());
        textInfo->setY(item->getY() - s * (0.5 * textInfo->getHeight() + 0.5 * item->getDiameter()) - 0.5 * textInfo->getHeight());

        _coutMutexPtr->lock();
        std::cout << _socketId << " " << _requestId << " " << map_server::TEXT
            << " {\"t\":\"" << textInfo->getText()
            << "\",\"e\":\"" << item->getElementId()
            << "\",\"x\":" << textInfo->getX() - textInfo->getXOffset()
            << ",\"y\":" << textInfo->getY() + textInfo->getYOffset() + textInfo->getHeight()
            << ",\"s\":" << textInfo->getFontSize()
            << ",\"z\":" << textInfo->getZIndex()
            << ",\"a\":" << textInfo->getAlpha()
            << ",\"r\":" << textInfo->getRed()
            << ",\"g\":" << textInfo->getGreen()
            << ",\"b\":" << textInfo->getBlue()
            << "}" << std::endl;
        _coutMutexPtr->unlock();

        /*RepulsiveCenter center = new RepulsiveCenter(item.X + 0.5 * item.CurrentWidth, item.Y + 0.5 * item.CurrentHeight,
                                                     1.0, 0.0, 0.8 * item.CurrentWidth, 0.8 * item.CurrentHeight, 1.0, false, false, true);
        item.RepulsiveCenterList.Add(center);*/

        return true;
    }

    bool TextDisplayer::displayLineText(LineItemCopy *item, TextInfo *textInfo)
    {
        return false;
    }

    bool TextDisplayer::displayFilledPolygonText(FilledPolygonItemCopy *item, TextInfo *textInfo)
    {
        return false;
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

                // Todo: If a text is displayed for this item, add the potential generated by this text
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
                int R = static_cast<int>(R1 * R1 + R2 * R2);
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
