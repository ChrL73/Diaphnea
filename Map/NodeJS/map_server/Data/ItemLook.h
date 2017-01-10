#pragma once

namespace map_server
{
    class IMap;

    class ItemLook
    {
    private:
        const int _id;
        const int _zIndex;
        const int _alpha;
        const int _red;
        const int _green;
        const int _blue;
        const double _size;
        char _json[128];

    public:
        ItemLook(int id, int zIndex, int alpha, int red, int green, int blue, double size, IMap *iMap);

        int getId(void) const { return _id; }
        int getZIndex(void) const { return _zIndex; }
        int getAlpha(void) const { return _alpha; }
        int getRed(void) const { return _red; }
        int getGreen(void) const { return _green; }
        int getBlue(void) const { return _blue; }
        double getSize(void) const { return _size; }

        const char *getJson(void) const { return _json; }
    };
}
