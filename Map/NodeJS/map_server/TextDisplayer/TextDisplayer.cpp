#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"
#include "ClientInfo.h"
#include "RepulsiveCenter.h"
#include "TextDisplayerParameters.h"

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

    TextDisplayer::TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, double width, double height) :
		_parameters(parameters), _width(width), _height(height),
        _maxPotential(new Potential(std::numeric_limits<double>::max(), std::numeric_limits<double>::max())),
        _minPotential(new Potential(-std::numeric_limits<double>::max(), -std::numeric_limits<double>::max())),
		_softThreshold(new Potential(parameters->getSoftThresholdExcludingTerm(), parameters->getSoftThresholdNotExcludingTerm())),
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
            int R = static_cast<int>(R1 * R1 + R2 * R2);
            if (R < _parameters->getPotentialTableSize())
            {
                if (center->getExcluding()) potential.addExcludingTerm(center->getU0() * _parameters->getExcludingPotential(R));
                else potential.addNotExcludingTerm(center->getU0() * _parameters->getNotExcludingPotential(R));

                if (!potential.isAcceptable(*_softThreshold)) break;
            }
        }

        return std::move(potential);
    }
}
