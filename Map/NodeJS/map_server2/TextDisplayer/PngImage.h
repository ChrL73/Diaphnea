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
