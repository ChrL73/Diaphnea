/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#pragma once

#include <string>

namespace png
{
    typedef unsigned char png_byte;
    typedef png_byte byte;
    template<typename T> struct basic_rgb_pixel;
    typedef basic_rgb_pixel<byte> rgb_pixel;
    template<typename pixel> class pixel_buffer;
    template<typename pixel, typename pixel_buffer_type> class image;
}

namespace map_server
{
    class PngImage
    {
    private:
        png::image<png::rgb_pixel, png::pixel_buffer<png::rgb_pixel> > *_image;

    public:
        PngImage(int width, int height);
        ~PngImage();

        void setPixel(int x, int y, unsigned char r, unsigned char g, unsigned char b);
        void save(std::string fileName);
    };
}
