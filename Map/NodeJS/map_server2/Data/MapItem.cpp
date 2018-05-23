#include "MapItem.h"
#include "RepulsiveCenter.h"
#include "TextInfo.h"

namespace map_server
{
    MapItem::~MapItem()
    {
        int i, n = _repulsiveCenterVector.size();
        for (i = 0; i < n; ++i) delete _repulsiveCenterVector[i];

        n = _textInfoVector.size();
        for (i = 0; i < n; ++i) delete _textInfoVector[i];
    }
}
