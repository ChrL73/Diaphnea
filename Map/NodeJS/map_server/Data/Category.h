#pragma once

namespace map_server
{
    class Category
    {
    private:
        const int _id;
        const int _framingLevel;

    public:
        Category(int id, int framingLevel) : _id(id), _framingLevel(framingLevel) {}

        int getId(void) const { return _id; }
        int getFramingLevel(void) const { return _framingLevel; }
    };
}
