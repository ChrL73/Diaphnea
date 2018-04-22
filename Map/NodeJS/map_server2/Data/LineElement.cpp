#include "LineElement.h"
#include "MapData.h"

#include <cstring>

namespace map_server
{
    typedef struct { int _id; } LineElementData;

    LineElement::LineElement(int offset)
    {
        const LineElementData *data = reinterpret_cast<LineElementData *>(lineElements + offset);

        const char *id = strings + data->_id;
        _id = new char[strlen(id) + 1];
        strcpy(_id, id);
    }
}
