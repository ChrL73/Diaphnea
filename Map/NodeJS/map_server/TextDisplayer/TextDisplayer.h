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

        const double _width;
        const double _height;

        std::vector<ItemCopy *> _itemVector;

        static constexpr double _maxRepulsionRatio = 5.0;
        static constexpr int _potentialTableSize = 1000;
		static constexpr int _maxVisibleTextCount = 100;
        const Potential * const _maxPotential;
        const Potential * const _minPotential;

        double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
        int getPotentialTableSize(void) const { return _potentialTableSize; }
        const Potential *getMaxPotential(void) const { return _minPotential; }
        const Potential *getMinPotential(void) const { return _maxPotential; }

        Potential getElementaryPotential(ItemCopy *item, double x, double y);

    public:
		static void clearClientMap(void);

        TextDisplayer(const std::string& socketId, double width, double height);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
