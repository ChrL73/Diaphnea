#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"
#include "ClientInfo.h"
#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"
#include "PngImage.h"

#include <limits>

namespace map_server
{
	std::mutex TextDisplayer::_mutex;
	std::map<std::string, ClientInfo *> TextDisplayer::_clientMap;

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

    TextDisplayer::TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, double width, double height, bool createPotentialImage) :
		_parameters(parameters), _width(width), _height(height), _createPotentialImage(createPotentialImage),
        _maxPotential(new Potential(std::numeric_limits<double>::max(), std::numeric_limits<double>::max())),
        _minPotential(new Potential(-std::numeric_limits<double>::max(), -std::numeric_limits<double>::max())),
		_softThreshold(new Potential(parameters->getSoftThreshold(), parameters->getSoftThreshold())),
		_hardThreshold(new Potential(parameters->getHardThresholdExcludingTerm(), parameters->getHardThresholdNotExcludingTerm()))
    {
		_mutex.lock();

		std::map<std::string, ClientInfo *>::iterator it = _clientMap.find(socketId);
		if (it == _clientMap.end())
		{
			it = _clientMap.insert(std::pair<std::string, ClientInfo *>(socketId, new ClientInfo())).first;
		}

		_clientInfo = (*it).second;

		_mutex.unlock();
    }

	TextDisplayer::~TextDisplayer()
	{
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i) delete _itemVector[i];

		delete _maxPotential;
		delete _minPotential;
		delete _softThreshold;
		delete _hardThreshold;
	}

    void TextDisplayer::start(void)
    {
		int visibleTextCount = 0;

		int i, n = _itemVector.size();
		for (i = 0; i < n && visibleTextCount < _parameters->getMaxVisibleTextCount(); ++i)
		{
			ItemCopy *item = _itemVector[i];
			TextInfo *textInfo = 0;
			if (item->getTextInfo1() != 0) textInfo = item->getTextInfo1();
			else if (item->getTextInfo2() != 0) textInfo = item->getTextInfo2();

			if (textInfo != 0)
			{

			}
		}

        if (_createPotentialImage)
        {
            PngImage image1(_width, _height);
            PngImage image2(_width, _height);
            PngImage image3(_width, _height);
            int j;
            for (i = 0; i < _width; ++i)
            {
                for (j = 0; j < _height; ++j)
                {
                    Potential p = getPotential(i, j, 0);

                    double value1 = p.getExcludingTerm() / _parameters->getSoftThreshold();
                    if (value1 >= 1.0)
                    {
                        image1.setPixel(i, j, 0, 0, 0);
                    }
                    else
                    {
                        if (value1 < 0.0) value1 = 0.0;
                        double r, g, b;
                        hsvToRgb(240.0 * (1.0 - value1), value1, 1.0, r, g, b);
                        image1.setPixel(i, j, 255 * r, 255 * g, 255 * b);
                    }


                    double value2 = p.getNotExcludingTerm() / _parameters->getSoftThreshold();
                    if (value2 >= 1.0)
                    {
                        image2.setPixel(i, j, 0, 0, 0);
                    }
                    else
                    {
                        if (value2 < 0.0) value2 = 0.0;
                        double r, g, b;
                        hsvToRgb(240.0 * (1.0 - value2), value2, 1.0, r, g, b);
                        image2.setPixel(i, j, 255 * r, 255 * g, 255 * b);
                    }

                    double value3 = value1 > value2 ? value1 : value2;
                    if (value3 >= 1.0)
                    {
                        image3.setPixel(i, j, 0, 0, 0);
                    }
                    else
                    {
                        if (value3 < 0.0) value3 = 0.0;
                        double r, g, b;
                        hsvToRgb(240.0 * (1.0 - value3), value3, 1.0, r, g, b);
                        image3.setPixel(i, j, 255 * r, 255 * g, 255 * b);
                    }
                }
            }

            image1.save("potential1.png");
            image2.save("potential2.png");
            image3.save("potential3.png");
        }
    }

    Potential TextDisplayer::getPotential(double x, double y, ItemCopy *selfItem)
    {
        Potential potential;

        const double u0 = _parameters->getEdgeRefPotential();
        const double epsilon = _parameters->getEdgeRangeRatio();
        double a;

        a = x / (epsilon * _width) - 1;
        if (a < 0) potential.addExcludingTerm(u0 * a * a);

        a = y / (epsilon * _height) - 1;
        if (a < 0) potential.addExcludingTerm(u0 * a * a);

        a = (x - _width) / (epsilon * _width) + 1;
        if (a > 0) potential.addExcludingTerm(u0 * a * a);

        a = (y - _height) / (epsilon * _height) + 1;
        if (a > 0) potential.addExcludingTerm(u0 * a * a);

        int i, n = _itemVector.size();
        for (i = 0; i < n; ++i)
        {
            ItemCopy *item = _itemVector[i];
            if (selfItem == 0 || item != selfItem)
            {
                Potential elementaryPotential = std::move(getElementaryPotential(item, x, y));
                potential += elementaryPotential;
                if (_clientInfo->isStopRequested() || !potential.isAcceptable(*_softThreshold)) return std::move(potential);

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
                    potential.addExcludingTerm(center->getU0() * _parameters->getExcludingPotential(R));

                    if (!potential.isAcceptable(*_softThreshold)) break;
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

                potential.addNotExcludingTerm(center->getU0() * p);

                if (!potential.isAcceptable(*_softThreshold)) break;
            }
        }

        return std::move(potential);
    }

    void TextDisplayer::hsvToRgb(double h, double s, double v, double& r, double& g, double& b)
    {
        double hh, p, q, t, ff;
        long i;

        if(s <= 0.0)
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

        switch(i) {
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

        case 5:
        default:
            r = v;
            g = p;
            b = q;
            break;
        }

        return;
    }
}
