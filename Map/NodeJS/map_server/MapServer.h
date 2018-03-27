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
        void checkTimeout(bool immediateReturn);

        void inputLoop(void);
        void processRequest(std::string requestString);

        const bool _softExit;
        bool _stopRequested;

        const std::string _dbHost;
        const std::string _dbName;
        const std::string _dbUser;
        const std::string _dbPassword;

    public:
        MapServer(time_t timeoutInSeconds, int cleanThreadSleepMs, int checkTimeoutSleepMs, bool softExit,
                  const std::string& dbHost, const std::string& dbName, const std::string& dbUser, const std::string& dbPassword) :
            _timeoutInSeconds(timeoutInSeconds), _cleanThreadSleepMs(cleanThreadSleepMs), _checkTimeoutSleepMs(checkTimeoutSleepMs),
            _softExit(softExit), _stopRequested(false), _dbHost(dbHost), _dbName(dbName), _dbUser(dbUser), _dbPassword(dbPassword) {}

        int run(void);
    };
}
