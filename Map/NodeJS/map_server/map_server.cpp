#include "MapServer.h"
#include <cstring>
#include <string>

int main(int argc, char *argv[])
{
	bool softExit = false;
	std::string dbHost, dbName, dbUser, dbPassword;

	int i, j = 0;
	for (i = 1; i < argc; ++i)
	{
        if (strcmp(argv[i], "softExit") == 0)
        {
            softExit = true;
        }
        else
        {
            if (j == 0) dbHost = argv[i];
            else if (j == 1) dbName = argv[i];
            else if (j == 2) dbUser = argv[i];
            else if (j == 3) dbPassword = argv[i];
            ++j;
        }
	}

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInSeconds = 300;

    map_server::MapServer mapServer(timeoutInSeconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit, dbHost, dbName, dbUser, dbPassword);

    int result = mapServer.run();

    return result;
}
