#include "PngImage.h"

#ifndef BUILD_WITHOUT_PNGPP
#include <png++/png.hpp>
#endif

namespace map_server
{
    PngImage::PngImage(int width, int height)
    {
        #ifndef BUILD_WITHOUT_PNGPP
        _image = new png::image<png::rgb_pixel, png::pixel_buffer<png::rgb_pixel> >(width, height);
        #endif
    }

    PngImage::~PngImage()
    {
        #ifndef BUILD_WITHOUT_PNGPP
        delete _image;
        #endif
    }

    void PngImage::setPixel(int x, int y, unsigned char r, unsigned char g, unsigned char b)
    {
        #ifndef BUILD_WITHOUT_PNGPP
        (*_image)[y][x] = png::rgb_pixel(r, g, b);
        #endif
    }

    void PngImage::save(std::string fileName)
    {
        #ifndef BUILD_WITHOUT_PNGPP
        _image->write(fileName);
        #endif
    }
}
