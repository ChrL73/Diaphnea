#include "GetMapIdsRequest.h"
#include "MapData.h"

namespace map_server
{
    void GetMapIdsRequest::execute(void)
    {
        MapData *mapData = MapData::instance();

        const std::vector<std::string>& mapIdVector = mapData->getMapIdVector();
    }
}
