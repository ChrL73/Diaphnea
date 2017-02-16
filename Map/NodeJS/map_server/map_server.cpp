#include "MapServer.h"
#include <cstring>

int main(int argc, char *argv[])
{
	bool softExit = false;

	int i;
	for (i = 1; i < argc; ++i)
	{
        if (strcmp(argv[i], "softExit") == 0) softExit = true;
	}

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInSeconds = 90;

    map_server::MapServer mapServer(timeoutInSeconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit);

    int result = mapServer.run();

    return result;
}
