#include "MapServer.h"
#include "ThreadInfo.h"
#include "Request.h"
#include "MapData.h"
#include "TextDisplayer.h"

#include <iostream>
#include <vector>
#include <cstring>

#include <thread>
#include <future>

namespace map_server
{
    int MapServer::run(void)
    {
        Request::setCoutMutex(&_coutMutex);
        TextDisplayer::setCoutMutex(&_coutMutex);

        MapData *mapData = MapData::instance();
        if (mapData == 0)
        {
            std::cerr << "Error in MapData::instance()" << std::endl;
            return -1;
        }

        _timeoutReference = time(0);

        std::thread deleteThread(&MapServer::cleanThreads, this);
        std::thread timeoutThread(&MapServer::checkTimeout, this, _softExit);
		if (_softExit) timeoutThread.join();

        inputLoop();

		if (!_softExit) exit(0);

		_threadSetMutex.lock();
		_stopRequested = true;
		_threadSetMutex.unlock();

		deleteThread.join();

		if (MapData::destroyInstance() != 0)
		{
			std::cerr << "Error in MapData::destroyInstance()" << std::endl;
			return -1;
		}

		TextDisplayer::clearClientMap();

		return 0;
    }

    void MapServer::inputLoop(void)
    {
        while(true)
        {
            std::string request;
            std::getline(std::cin, request);

            // When the process was spawned by node and node crashes, 'getline' returns and request size is 0
            if (request.size() == 0) break;

            _timeMutex.lock();
            _timeoutReference = time(0);
            _timeMutex.unlock();

            std::packaged_task<void(std::string)> task(std::bind(&MapServer::processRequest, this, std::placeholders::_1));
            std::future<void> future = task.get_future();
            std::thread t(std::move(task), request);

            ThreadInfo *threadInfo = new ThreadInfo(std::move(t), std::move(future), std::move(request));

            _threadSetMutex.lock();
            _threadSet.insert(threadInfo);
            _threadSetMutex.unlock();
        }
    }

    void MapServer::cleanThreads(void)
    {
        std::vector<std::set<ThreadInfo *>::iterator> threadsToDelete;

        while(true)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(_cleanThreadSleepMs));

            _threadSetMutex.lock();

            std::set<ThreadInfo *>::iterator it = _threadSet.begin();
            for (; it != _threadSet.end(); ++it)
            {
                std::future_status status = (*it)->getF().wait_for(std::chrono::milliseconds(0));
                if (status == std::future_status::ready) threadsToDelete.push_back(it);
            }

            int i, n = threadsToDelete.size();
            for (i = 0; i < n; ++i)
            {
                std::set<ThreadInfo *>::iterator it = threadsToDelete[i];
                (*it)->getT().join();

                delete (*it);
                _threadSet.erase(it);
            }

            threadsToDelete.clear();

			if (_stopRequested && _threadSet.empty())
			{
				_threadSetMutex.unlock();
				break;
			}

            _threadSetMutex.unlock();
        }
    }

    void MapServer::checkTimeout(bool immediateReturn)
    {
        if (immediateReturn) return;

        while (true)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(_checkTimeoutSleepMs));

            _timeMutex.lock();
            if (time(0) - _timeoutReference > _timeoutInSeconds)
            {
                _timeMutex.unlock();
				exit(0);
            }
            _timeMutex.unlock();
        }
    }

    void MapServer::processRequest(std::string requestString)
    {
        std::vector<const char *> tokenVector;
		int i, n = requestString.size() + 1;
        char *req = new char[n];

#if _WIN32
        strcpy_s(req, n, requestString.c_str());
#else
        strcpy(req, requestString.c_str());
#endif

        char *tokenStart = req;
        for (i = 0; i < n; ++i)
        {
            if (req[i] == ' ' || req[i] == 0)
            {
                req[i] = 0;
                if (i != 0 && req[i - 1] != 0) tokenVector.push_back(tokenStart);
                tokenStart = req + i + 1;
            }
        }

        Request *request = Request::createRequest(tokenVector);

        if (request != 0)
        {
            request->execute();
        }
        else
        {
            _coutMutex.lock();
            std::cerr << "Incorrect request" << std::endl; // Todo: Handle this error in Node JS
            _coutMutex.unlock();
        }

        delete[] req;
        delete request;
    }
}
