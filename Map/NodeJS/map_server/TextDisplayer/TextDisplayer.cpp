#include "TextDisplayer.h"
#include "ItemCopy.h"
#include "Potential.h"
#include "ClientInfo.h"

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

    TextDisplayer::TextDisplayer(const std::string& socketId, double xMin, double xMax, double yMin, double yMax) : _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax),
        _maxRepulsionRatio(5.0), _potentialTableSize(10000), _maxVisibleTextCount(100),
        _maxPotential(new Potential(std::numeric_limits<double>::max(), std::numeric_limits<double>::max())),
        _minPotential(new Potential(-std::numeric_limits<double>::max(), -std::numeric_limits<double>::max()))
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
	}

    void TextDisplayer::start(void)
    {
		int visibleTextCount = 0;

		int i, n = _itemVector.size();
		for (i = 0; i < n && visibleTextCount < _maxVisibleTextCount; ++i)
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
}
