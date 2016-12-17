#pragma once

#include <vector>

namespace map_server
{
    class RepulsiveCenter;
	class TextInfo;

    class ItemCopy
    {
    private:
        std::vector<const RepulsiveCenter *> _repulsiveCenterVector;
		TextInfo *_textInfo;

    public:
		ItemCopy(void) : _textInfo(0) {}
		virtual ~ItemCopy();

        void addRepulsiveCenter(RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }

    };
}

