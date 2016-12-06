#include "Look.h"
#include "ItemLook.h"

namespace map_server
{
    Look::~Look()
    {
        delete _textLook;
    }
}
