#pragma once

#include "IDisplayerConstants.h"

#include <vector>
#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class ItemCopy;
	class ClientInfo;

    class TextDisplayer : public IDisplayerConstants
    {
    private:
		static std::mutex _mutex;
		static std::map<std::string, ClientInfo *> _clientMap;
		ClientInfo *_clientInfo;

        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        std::vector<ItemCopy *> _itemVector;

        const double _maxRepulsionRatio;
        const int _potentialTableSize;
		const int _maxVisibleTextCount;
        const Potential * const _maxPotential;
        const Potential * const _minPotential;

        double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
        int getPotentialTableSize(void) const { return _potentialTableSize; }
        const Potential *getMaxPotential(void) const { return _minPotential; }
        const Potential *getMinPotential(void) const { return _maxPotential; }

    public:
		static void clearClientMap(void);

        TextDisplayer(const std::string& socketId, double xMin, double xMax, double yMin, double yMax);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
