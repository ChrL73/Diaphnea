#pragma once

#include "ElementTypeEnum.h"

namespace map_server
{
    class ElementInfo
    {
    private:
        const ElementTypeEnum _type;
        const int _offset;

    public:
        ElementInfo(ElementTypeEnum type, int offset) : _type(type), _offset(offset) {}

        ElementTypeEnum getType(void) const { return _type; }
        int getOffset(void) const { return _offset; }
    };
}
