#pragma once

namespace map_server
{
    enum RequestTypeEnum
    {
        GET_MAP_IDS = 0,
        GET_MAP_INFO = 1,
        GET_ELEMENT_INFO = 2,
        GET_ELEMENTS_INFO = 3,
        RENDER = 4
    };
}