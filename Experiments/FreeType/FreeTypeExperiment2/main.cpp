#include "ft2build.h"
#include FT_FREETYPE_H

#include <iostream>
#include <string>

int wmain(int argc, wchar_t *argv[])
{
    if (argc < 4)
    {
        std::cout << "{\"message\":\"Not enough arguments\"}";
        return -1;
    }

    std::wstring text;
    int i, wordCount = argc - 3;
    for (i = 0; i < wordCount; ++i)
    {
        if (i != 0) text += L" ";
        text += argv[i + 1];
    }

    std::wstring fontFamily0(argv[wordCount + 2]);
    std::string fontFamily;
    int n = fontFamily0.size();
    for (i = 0; i < n; ++i) fontFamily += fontFamily0[i];

#ifdef _WIN32
    std::string fontFile = fontFamily + ".ttf";
#else
    std::string fontFile = "/usr/share/fonts/truetype/msttcorefonts/" + fontFamily + ".ttf";
#endif

    double fontSize;
    try
    {
        fontSize = std::stod(argv[wordCount + 1]);
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

    error = FT_New_Face(library, fontFile.c_str(), 0, &face);

    if (error == FT_Err_Unknown_File_Format)
    {
        std::cout << "{\"message\":\"FT_New_Face error: Unknown file format\"}";
        return -1;
    }
    else if (error)
    {
        std::cout << "{\"message\":\"FT_New_Face error: Fail to open font file " + fontFile + "\"}";
        return -1;
    }

    error = FT_Set_Pixel_Sizes(face, 0, static_cast<FT_UInt>(fontSize));

    if (error)
    {
        std::cout << "{\"message\":\"FT_Set_Pixel_Sizes error\"}";
        return -1;
    }

    n = text.size();
    int width = 0, yMaxMax = 0, yMinMin = 0, left = 0;

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

    std::cout << "{\"message\":\"OK\",\"width\":" << static_cast<double>(width) / 64.0
        << ",\"height\":" << static_cast<double>(yMaxMax - yMinMin) / 64.0
        << ",\"left\":" << static_cast<double>(left) / 64.0 << ",\"bottom\":" << static_cast<double>(yMinMin) / 64.0 << "}";

    return 0;
}
