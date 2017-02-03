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

    public:
        SvgTextInfo(const TextInfo *textInfo) : _textInfo(textInfo) {}

        void addLine(const SvgLineInfo *line) { _lineVector.push_back(line); }

        const std::vector<const SvgLineInfo *>& getLineVector(void) const { return _lineVector; }
        const TextInfo *getTextInfo(void) const { return _textInfo; }
    };
}
