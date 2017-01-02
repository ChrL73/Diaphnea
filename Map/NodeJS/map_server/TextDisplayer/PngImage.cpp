#include "PngImage.h"
#include <png++/png.hpp>

namespace map_server
{
    PngImage::PngImage(int width, int height)
    {
        _image = new png::image<png::rgb_pixel, png::pixel_buffer<png::rgb_pixel> >(width, height);
    }

    PngImage::~PngImage()
    {
        delete _image;
    }

    void PngImage::setPixel(int x, int y, unsigned char r, unsigned char g, unsigned char b)
    {
        (*_image)[y][x] = png::rgb_pixel(r, g, b);
    }

    void PngImage::save(const char *fileName)
    {
        _image->write(fileName);
    }
}
