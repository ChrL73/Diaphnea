#pragma once

#include <string>
#include <set>
#include <ctime>

#include <mutex>

namespace map_server
{
    class ThreadInfo;
    class CommonData;

    class MapServer
    {
    private:
        CommonData * const _commonData;

        std::set<ThreadInfo *> _threadSet;
        std::mutex _threadSetMutex;

        time_t _timeoutReference;
        std::mutex _timeMutex;
        const time_t _timeoutInSeconds;

        std::mutex _coutMutex;

        const int _cleanThreadSleepMs;
        void cleanThreads(void);

        const int _checkTimeoutSleepMs;
        void checkTimeout(bool immediateReturn);

        void inputLoop(void);
        void processRequest(std::string requestString);

        const bool _softExit;
        bool _stopRequested;

    public:
        MapServer(time_t timeoutInSeconds, int cleanThreadSleepMs, int checkTimeoutSleepMs, bool softExit);
        ~MapServer();

        int run(void);
    };
}
