#include "MapServer.h"
#include <cstring>
#include <string>

int main(int argc, char *argv[])
{
    bool softExit = false;
    if (argc >= 2 && strcmp(argv[1], "softExit") == 0) softExit = true;

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInSeconds = 300;

    map_server::MapServer mapServer(timeoutInSeconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit);

    int result = mapServer.run();

    return result;
}
