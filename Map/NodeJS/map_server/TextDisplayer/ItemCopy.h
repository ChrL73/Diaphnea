#pragma once

#include <vector>
#include <string>

namespace map_server
{
    class RepulsiveCenter;
	class TextInfo;

    class ItemCopy
    {
    private:
        std::vector<const RepulsiveCenter *> _repulsiveCenterVector;
		std::vector<TextInfo *> _textInfoVector;
		std::string _elementId;

    public:
		ItemCopy(const std::string& elementId) : _elementId(elementId) {}
		virtual ~ItemCopy();

        void addRepulsiveCenter(RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }
        const std::vector<const RepulsiveCenter *>& getRepulsiveCenterVector(void) const { return _repulsiveCenterVector; }

        void addTextInfo(TextInfo *textInfo) { _textInfoVector.push_back(textInfo); }
		int getTextInfoCount(void) const { return _textInfoVector.size(); }
		TextInfo *getTextInfo(int i) const { return _textInfoVector[i]; }
		const std::string& getElementId(void) const { return _elementId; }
    };
}
