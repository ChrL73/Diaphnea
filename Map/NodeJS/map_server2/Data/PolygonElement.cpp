#include "PolygonElement.h"
#include "MapData.h"

#include <cstring>

namespace map_server
{
    typedef struct { int _id; } PolygonElementData;

    PolygonElement::PolygonElement(int offset)
    {
        const PolygonElementData *data = reinterpret_cast<PolygonElementData *>(polygonElements + offset);

        const char *id = strings + data->_id;
        _id = new char[strlen(id) + 1];
        strcpy(_id, id);
    }
}
