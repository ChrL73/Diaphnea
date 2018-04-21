#pragma once

namespace map_server
{
    enum ErrorEnum
    {
        UNHANDLED_EXCEPTION = 0,
        EMPTY_REQUEST = 6,
        NO_REQUEST_ID = 7,
        NO_REQUEST_TYPE = 8,
        BAD_REQUEST_TYPE = 9,
        NOT_ENOUGH_TOKENS = 10,
        BAD_PARAMETER = 11,
        UNKNOWN_ID = 12,
        FREE_TYPE_INIT_FAILED = 13,
        FONT_NOT_FOUND = 14
    };
}
