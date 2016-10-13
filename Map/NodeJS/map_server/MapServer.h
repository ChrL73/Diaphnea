#pragma once

#include <string>
#include <set>
#include <ctime>

#include <mutex>

namespace map_server
{
    class ThreadInfo;

    class MapServer
    {
    private:
        std::set<ThreadInfo *> _threadSet;
        std::mutex _threadSetMutex;

        time_t _timeoutReference;
        std::mutex _timeMutex;
        const time_t _timeoutInSeconds;

        std::mutex _coutMutex;

        const int _cleanThreadSleepMs;
        void cleanThreads(void);

        const int _checkTimeoutSleepMs;
        void checkTimeout(void);

        void processRequest(std::string request);
        int exitProcess(void);

    public:
        MapServer(time_t timeoutInSeconds, int cleanThreadSleepMs, int checkTimeoutSleepMs) :
            _timeoutInSeconds(timeoutInSeconds), _cleanThreadSleepMs(cleanThreadSleepMs), _checkTimeoutSleepMs(checkTimeoutSleepMs) {}

        int run(void);
    };
}
