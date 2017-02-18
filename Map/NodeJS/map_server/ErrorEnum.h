#pragma once

namespace map_server
{
	enum ErrorEnum
	{
		EMPTY_REQUEST = 0,
		NO_REQUEST_ID = 1,
		NO_REQUEST_TYPE = 2,
		BAD_REQUEST_TYPE = 3,
		NOT_ENOUGH_TOKENS = 4,
		BAD_PARAMETER = 5
	};
}
