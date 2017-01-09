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
		TextInfo *_textInfo1;
		TextInfo *_textInfo2;

    public:
		ItemCopy(void) : _textInfo1(0), _textInfo2(0) {}
		virtual ~ItemCopy();

        void addRepulsiveCenter(RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }
        const std::vector<const RepulsiveCenter *>& getRepulsiveCenterVector(void) const { return _repulsiveCenterVector; }

        void setTextInfo1(TextInfo *textInfo1) { _textInfo1 = textInfo1; }
        void setTextInfo2(TextInfo *textInfo2) { _textInfo2 = textInfo2; }
		TextInfo *getTextInfo1(void) const { return _textInfo1; }
		TextInfo *getTextInfo2(void) const { return _textInfo2; }
    };
}
