#include "MapServer.h"
#include <cstring>

int main(int argc, char *argv[])
{
	bool softExit = false;
	bool mutexTest = false;

	int i;
	for (i = 1; i < argc; ++i)
	{
        if (strcmp(argv[i], "softExit") == 0) softExit = true;
        if (strcmp(argv[i], "mutexTest") == 0) mutexTest = true;
	}

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInseconds = 60;

    map_server::MapServer mapServer(timeoutInseconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit, mutexTest);

    int result = mapServer.run();

    return result;
}
