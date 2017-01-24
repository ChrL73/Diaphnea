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
		const std::string _elementId;
		const double _importance;

    public:
		ItemCopy(const std::string& elementId, double importance) : _elementId(elementId), _importance(importance) {}
		virtual ~ItemCopy();

        void addRepulsiveCenter(RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }
        const std::vector<const RepulsiveCenter *>& getRepulsiveCenterVector(void) const { return _repulsiveCenterVector; }

        void addTextInfo(TextInfo *textInfo) { _textInfoVector.push_back(textInfo); }
		int getTextInfoCount(void) const { return _textInfoVector.size(); }
		TextInfo *getTextInfo(int i) const { return _textInfoVector[i]; }
		const std::string& getElementId(void) const { return _elementId; }
		double getImportance(void) const { return _importance; }
    };
}
