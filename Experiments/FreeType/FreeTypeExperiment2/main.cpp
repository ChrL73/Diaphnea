#include "ft2build.h"
#include FT_FREETYPE_H

#include <iostream>
#include <string>

int main(int argc, char *argv[])
{
	if (argc < 4)
	{
		std::cout << "{\"message\":\"Not enough arguments\"}";
		return -1;
	}

	std::string text(argv[1]);
	std::string fontFamily(argv[3]);
	std::string fontFile = fontFamily + ".ttf";

	double fontSize;
	try
	{
		fontSize = std::stod(argv[2]);
	}
	catch (...)
	{
		std::cout << "{\"message\":\"Bad font size\"}";
		return -1;
	}

	if (fontSize < 1.0 || fontSize > 999.0)
	{
		std::cout << "{\"message\":\"Bad font size\"}";
		return -1;
	}

    FT_Library  library;
    int error = FT_Init_FreeType(&library);

    if (error)
    {
		std::cout << "{\"message\":\"FT_Init_FreeType error\"}";
        return -1;
    }

    FT_Face face;

    //error = FT_New_Face(library, "/usr/share/fonts/truetype/msttcorefonts/Arial.ttf", 0, &face);
	error = FT_New_Face(library, fontFile.c_str(), 0, &face);

    if (error == FT_Err_Unknown_File_Format)
    {
        std::cout << "{\"message\":\"FT_New_Face error: Unknown file format\"}";
        return -1;
    }
    else if (error)
    {
		std::cout << "{\"message\":\"FT_New_Face error: Fail to open font file\"}";
        return -1;
    }

    error = FT_Set_Pixel_Sizes(face, 0, static_cast<FT_UInt>(fontSize));

    if (error)
    {
        std::cout << "{\"message\":\"FT_Set_Pixel_Sizes error\"}";
        return -1;
    }

	int i, n = text.size();

    for (i = 0; i < n; ++i)
    {
        FT_UInt glyph_index;

        glyph_index = FT_Get_Char_Index(face, text[i]);

        error = FT_Load_Glyph(face, glyph_index, FT_LOAD_DEFAULT);
        if (error)
		{
			std::cout << "{\"message\":\"FT_Load_Glyph error\"}";
			return -1;
		}

        error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_MONO);
		if (error)
		{
			std::cout << "{\"message\":\"FT_Render_Glyph error\"}";
			return -1;
		}

        std::cout << "i: " << i
            << " text[i]: " << text[i]
            << " face->glyph->bitmap_left: " << face->glyph->bitmap_left
            << " face->glyph->bitmap_top: " << face->glyph->bitmap_top
            << " face->glyph->advance.x: " << face->glyph->advance.x
            << " face->glyph->advance.y: " << face->glyph->advance.y
            << std::endl;
    }

    return 0;
}
