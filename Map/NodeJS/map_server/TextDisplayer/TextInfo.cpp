#include "TextInfo.h"

#include "ft2build.h"
#include FT_FREETYPE_H

namespace map_server
{
    TextInfo::TextInfo(double size, double scale, const std::string& text) : _text(text), _ok(false)
    {
		double fontSize = size * scale;
		if (fontSize < 1.0 || fontSize > 999.0) return;

#ifdef _WIN32
		std::string fontFile = "arial.ttf";
#else
		std::string fontFile = "/usr/share/fonts/truetype/msttcorefonts/arial.ttf";
#endif

		// Todo: Call FT_Init_FreeType, FT_New_Face and FT_Done_FreeType only once by request
		FT_Library  library;
		int error = FT_Init_FreeType(&library);
		if (error) return;

		FT_Face face;
		error = FT_New_Face(library, fontFile.c_str(), 0, &face);
		if (error) return;

		error = FT_Set_Pixel_Sizes(face, 0, static_cast<FT_UInt>(fontSize));
		if (error) return;

		int i, n = text.size();
		int width = 0, yMaxMax = 0, yMinMin = 0, left = 0;

		for (i = 0; i < n; ++i)
		{
			FT_UInt glyph_index;
			glyph_index = FT_Get_Char_Index(face, text[i]);

			error = FT_Load_Glyph(face, glyph_index, FT_LOAD_DEFAULT);
			if (error) return;

			error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_MONO);
			if (error) return;

			if (i == 0)
			{
				left = face->glyph->metrics.horiBearingX;

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

		FT_Done_FreeType(library);

		_geoWidth = static_cast<double>(width) / (64.0 * scale);
		_geoHeight = static_cast<double>(yMaxMax - yMinMin) / (64.0 * scale);
		_leftInPixels = static_cast<double>(left) / 64.0;
		_bottomInPixels = static_cast<double>(yMinMin) / 64.0;

		_ok = true;
    }
}
