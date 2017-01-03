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
        const bool _createPotentialImage;

        std::vector<ItemCopy *> _itemVector;

        const Potential * const _maxPotential;
        const Potential * const _minPotential;
		const Potential * const _softThreshold;
		const Potential * const _hardThreshold;

        Potential getPotential(double x, double y, ItemCopy *selfItem);
        Potential getElementaryPotential(ItemCopy *item, double x, double y);

        void hsvToRgb(double h, double s, double v, double& r, double& g, double& b);

    public:
		static void clearClientMap(void);

        TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, double width, double height, bool createPotentialImage);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
