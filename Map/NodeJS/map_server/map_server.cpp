#include "MapServer.h"

int main(int argc, char *argv[])
{
	bool softExit = (argc > 1 && strcmp(argv[1], "softExit") == 0);

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInseconds = 60;

    map_server::MapServer mapServer(timeoutInseconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit);

    int result = mapServer.run();

    return result;
}
