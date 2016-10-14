#include "MapServer.h"

int main()
{
    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInseconds = 60;

    map_server::MapServer mapServer(timeoutInseconds, cleanThreadSleepMs, checkTimeoutSleepMs);

    int result = mapServer.run();

    return result;
}
