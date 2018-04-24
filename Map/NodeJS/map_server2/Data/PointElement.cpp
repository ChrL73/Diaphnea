#include "PointElement.h"
#include "MapData.h"

#include <cstring>

namespace map_server
{
    typedef struct { int _id; } PointElementData;

    PointElement::PointElement(int offset)
    {
        const PointElementData *data = reinterpret_cast<PointElementData *>(pointElements + offset);

        const char *id = strings + data->_id;
        _id = new char[strlen(id) + 1];
        strcpy(_id, id);
    }
}
