#include "ft2build.h"
#include FT_FREETYPE_H

#include <iostream>

int main()
{
    FT_Library  library;
    int error = FT_Init_FreeType(&library);

    if (error)
    {
        std::cout << "FT_Init_FreeType error !" << std::endl;
        return -1;
    }

    std::cout << "FT_Init_FreeType success !" << std::endl;

    FT_Face face;

    error = FT_New_Face(library, "/usr/share/fonts/truetype/msttcorefonts/Arial.ttf", 0, &face);

    if (error == FT_Err_Unknown_File_Format)
    {
        std::cout << "FT_New_Face: the font file could be opened and read, but it appears that its font format is unsupported" << std::endl;
        return -1;
    }
    else if ( error )
    {
        std::cout << "FT_New_Face: another error code means that the font file could not be opened or read, or that it is broken" << std::endl;
        return -1;
    }

    std::cout << "FT_New_Face success !" << std::endl;

    std::cout << "face−>num_glyphs: " << face->num_glyphs << std::endl;
    std::cout << "face−>face_flags: " << face->face_flags << std::endl;
    std::cout << "face−>units_per_EM: " << face->units_per_EM << std::endl;
    std::cout << "face−>num_fixed_sizes: " << face->num_fixed_sizes << std::endl;
    std::cout << "face−>available_sizes: " << face->available_sizes << std::endl;

    error = FT_Set_Pixel_Sizes(face, 0, 20);

    if (error)
    {
        std::cout << "FT_Set_Pixel_Sizes error !" << std::endl;
        return -1;
    }

    std::cout << "FT_Set_Pixel_Sizes success !" << std::endl;

    int charcode = 65;
    int glyph_index = FT_Get_Char_Index(face, charcode);
    std::cout << "glyph_index: " << glyph_index << std::endl;

    int load_flags = 0;
    error = FT_Load_Glyph(face, glyph_index, load_flags);

    if (error)
    {
        std::cout << "FT_Load_Glyph error !" << std::endl;
        return -1;
    }

    std::cout << "FT_Load_Glyph success !" << std::endl;

    error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL);

    if (error)
    {
        std::cout << "FT_Render_Glyph error !" << std::endl;
        return -1;
    }

    std::cout << "FT_Render_Glyph success !" << std::endl;

    std::cout << "face->glyph->bitmap_left: " << face->glyph->bitmap_left << std::endl;
    std::cout << "face->glyph->bitmap_top: " << face->glyph->bitmap_top << std::endl;

    FT_GlyphSlot  slot = face->glyph;
    int pen_x = 300, pen_y = 200, n;

    char text[] = "Bonjour";
    int num_chars = 7;

    for (n = 0; n < num_chars; n++)
    {
        FT_UInt glyph_index;

        /* retrieve glyph index from character code */
        glyph_index = FT_Get_Char_Index(face, text[n]);

        /* load glyph image into the slot (erase previous one) */
        error = FT_Load_Glyph(face, glyph_index, FT_LOAD_DEFAULT);
        if (error) continue;  /* ignore errors */

        /* convert to an anti-aliased bitmap */
        error = FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL);
        if (error) continue;

        /* now, draw to our target surface */
        //my_draw_bitmap(&slot->bitmap, pen_x + c, pen_y - slot->bitmap_top);

        std::cout << "n: " << n
            << " text[n]: " << text[n]
            << " slot->bitmap_left: " << slot->bitmap_left
            << " slot->bitmap_top: " << slot->bitmap_top
            << " slot->advance.x: " << slot->advance.x
            << " slot->advance.y: " << slot->advance.y
            << std::endl;

        /* increment pen position */
        pen_x += slot->advance.x >> 6;
        pen_y += slot->advance.y >> 6; /* not useful for now */
    }

    return 0;
}
