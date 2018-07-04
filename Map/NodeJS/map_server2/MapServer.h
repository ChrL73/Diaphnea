/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

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
