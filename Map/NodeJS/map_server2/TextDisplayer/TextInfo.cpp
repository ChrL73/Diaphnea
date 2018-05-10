#include "TextInfo.h"
#include "TextInfoLine.h"

#include "ft2build.h"
#include FT_FREETYPE_H

#include <locale>
#include <codecvt>

namespace map_server
{
    TextInfo::TextInfo(const std::vector<TextInfoLine *>& lineVector, FT_Face face, double fontSize) :
        _lineVector(lineVector), _width(0.0), _ok(false)
    {
        const double minFontSize = 5.0;
        const double maxFontSize = 100.0;
        if (fontSize < minFontSize || fontSize > maxFontSize) return;

        int error = FT_Set_Pixel_Sizes(face, 0, static_cast<FT_UInt>(fontSize));
        if (error) return;

        double firstYMaxMax = 0.0, lastYMinMin = 0.0;
        int k, lineCount = _lineVector.size();
        for (k = 0; k < lineCount; ++k)
        {
            TextInfoLine *line = _lineVector[k];
            std::wstring_convert<std::codecvt_utf8<wchar_t> > converter;
            std::wstring wtext = converter.from_bytes(line->getText());

            int i, n = wtext.size();
            int width = 0.0, yMaxMax = 0.0, yMinMin = 0.0, xOffset = 0.0;

            for (i = 0; i < n; ++i)
            {
                FT_UInt glyph_index;
                glyph_index = FT_Get_Char_Index(face, wtext[i]);

                error = FT_Load_Glyph(face, glyph_index, FT_LOAD_DEFAULT);
                if (error) return;

                error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_MONO);
                if (error) return;

                if (i == 0)
                {
                    xOffset = face->glyph->metrics.horiBearingX;

                    if (n == 1)
                    {
                        width += face->glyph->metrics.width;
                    }
                    else
                    {
                        width += face->glyph->advance.x - face->glyph->metrics.horiBearingX;
                    }
                }
                else if (i == n - 1)
                {
                    width += face->glyph->metrics.width + face->glyph->metrics.horiBearingX;
                }
                else
                {
                    width += face->glyph->advance.x;
                }

                if (face->glyph->metrics.horiBearingY > yMaxMax) yMaxMax = face->glyph->metrics.horiBearingY;

                int yMin = face->glyph->metrics.horiBearingY - face->glyph->metrics.height;
                if (yMin < yMinMin) yMinMin = yMin;
            }

            double w = static_cast<double>(width) / 64.0;
            line->setWidth(w);
            if (w > _width) _width = w;
            line->setXOffset(static_cast<double>(xOffset) / 64.0);
            line->setYOffset(static_cast<double>(yMinMin) / 64.0);

            if (k == 0) firstYMaxMax = yMaxMax;
            if (k == lineCount - 1) lastYMinMin = yMinMin;
        }

        int lineDistance = face->size->metrics.height;
        _height = static_cast<double>(firstYMaxMax - lastYMinMin + (lineCount - 1) * lineDistance) / 64.0;

        for (k = 0; k < lineCount; ++k)
        {
            TextInfoLine *line = _lineVector[k];
            line->setXOffset(line->getXOffset() - 0.5 * (_width - line->getWidth()));
            line->setYOffset(_lineVector[lineCount - 1]->getYOffset() - static_cast<double>((lineCount - 1 - k) * lineDistance) / 64.0);
        }

        _ok = true;
    }

    TextInfo::~TextInfo()
    {
        int i, n = _lineVector.size();
        for (i = 0; i < n; ++i) delete _lineVector[i];
    }
}
