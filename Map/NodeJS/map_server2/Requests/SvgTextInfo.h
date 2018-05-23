#pragma once

#include "SvgInfo.h"

#include <vector>

namespace map_server
{
    class SvgLineInfo;
    class TextInfo;

    class SvgTextInfo : public SvgInfo
    {
    private:
        std::vector<const SvgLineInfo *> _lineVector;
        const TextInfo * const _textInfo;
        const int _lookId;
        const char * const _color;
        const double _opacity;
        const double _fontSize;

    public:
        SvgTextInfo(const TextInfo *textInfo, int lookId, const char *color, double opacity, double fontSize) :
            _textInfo(textInfo), _lookId(lookId), _color(color), _opacity(opacity), _fontSize(fontSize) {}

        void addLine(const SvgLineInfo *line) { _lineVector.push_back(line); }

        const std::vector<const SvgLineInfo *>& getLineVector(void) const { return _lineVector; }
        const TextInfo *getTextInfo(void) const { return _textInfo; }
        int getLookId(void) const { return _lookId; }
        const char *getColor(void) const { return _color; }
        double getOpacity(void) const { return _opacity; }
        double getFontSize(void) const { return _fontSize; }
    };
}
