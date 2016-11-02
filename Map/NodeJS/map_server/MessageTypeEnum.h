#pragma once

namespace map_server
{
    enum MessageTypeEnum
    {
        GET_MAP_IDS = 0,
        GET_MAP_INFO = 1,
        GET_ELEMENT_INFO = 2,
        GET_ELEMENTS_INFO = 3,
        GET_ITEM_INFO = 4,
        GET_LOOK = 5,
        RENDER = 6
    };
}
