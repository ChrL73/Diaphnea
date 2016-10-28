#pragma once

#include <string>

namespace map_server
{
    class ItemLook;

    class Look
    {
    private:
        const int _id;
        const ItemLook * const _textLook;

    protected:
        Look(int id, const ItemLook *textLook) : _id(id), _textLook(textLook) {}

    public:
        virtual ~Look();

        int getId(void) const { return _id; }
        const ItemLook *getTextLook(void) const { return _textLook; }
    };
}
