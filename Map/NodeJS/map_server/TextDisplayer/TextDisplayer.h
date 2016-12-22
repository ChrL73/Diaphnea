#pragma once

#include "Potential.h"

#include <vector>
#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class ItemCopy;
	class ClientInfo;
	class TextDisplayerParameters;

    class TextDisplayer
    {
    private:
		static std::mutex _mutex;
		static std::map<std::string, ClientInfo *> _clientMap;
		ClientInfo *_clientInfo;

		const TextDisplayerParameters * const _parameters;
        const double _width;
        const double _height;

        std::vector<ItemCopy *> _itemVector;

        const Potential * const _maxPotential;
        const Potential * const _minPotential;
		const Potential * const _softThreshold;
		const Potential * const _hardThreshold;

        Potential getElementaryPotential(ItemCopy *item, double x, double y);

    public:
		static void clearClientMap(void);

        TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, double width, double height);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
