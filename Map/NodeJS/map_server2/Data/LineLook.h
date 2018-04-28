#pragma once

namespace map_server
{
    class LineLook
    {
    private:
        int _lineZIndex;
        int _lineAlpha;
        int _lineRed;
        int _lineGreen;
        int _lineBlue;
        double _lineSize;

        int _textZIndex;
        int _textAlpha;
        int _textRed;
        int _textGreen;
        int _textBlue;
        double _textSize;
       
        int _lookId;

    public:
        int getLineZIndex(void) const { return _lineZIndex; }
        int getLineAlpha(void) const { return _lineAlpha; }
        int getLineRed(void) const { return _lineRed; }
        int getLineGreen(void) const { return _lineGreen; }
        int getLineBlue(void) const { return _lineBlue; }
        double getLineSize(void) const { return _lineSize; }
        
        int getTextZIndex(void) const { return _textZIndex; }
        int getTextAlpha(void) const { return _textAlpha; }
        int getTextRed(void) const { return _textRed; }
        int getTextGreen(void) const { return _textGreen; }
        int getTextBlue(void) const { return _textBlue; }
        double getTextSize(void) const { return _textSize; }

        int getLookId(void) const { return _lookId; }
    };
}
