#include "MapServer.h"
#include "ThreadInfo.h"
#include "Request.h"
#include "MapData.h"

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

        MapData *mapData = MapData::instance();
        if (mapData == 0)
        {
            std::cerr << "Error in MapData::instance()" << std::endl;
            return -1;
        }

        std::thread deleteThread(&MapServer::cleanThreads, this);
        std::thread timeoutThread(&MapServer::checkTimeout, this);
        _timeoutReference = time(0);

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

        return exitProcess();
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

            _threadSetMutex.unlock();
        }
    }

    void MapServer::checkTimeout(void)
    {
        while (true)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(_checkTimeoutSleepMs));

            _timeMutex.lock();
            if (time(0) - _timeoutReference > _timeoutInSeconds)
            {
                _timeMutex.unlock();
                exitProcess();
            }
            _timeMutex.unlock();
        }
    }

    void MapServer::processRequest(std::string requestString)
    {
        std::vector<const char *> tokenVector;
		int n = requestString.size() + 1;
        char *req = new char[n];
        strcpy_s(req, n, requestString.c_str());
		char *context = 0;
        char *token = strtok_s(req, " ", &context);
        while (token != 0)
        {
            tokenVector.push_back(token);
            token = strtok_s(0, " ", &context);
        }

        Request *request = Request::createRequest(tokenVector);

        if (request != 0)
        {
            request->execute();
        }
        else
        {
            _coutMutex.lock();
            std::cerr << "Incorrect request" << std::endl; // Todo: Handle this error on Node JS
            _coutMutex.unlock();
        }

        delete[] req;
        delete request;

        /*std::this_thread::sleep_for(std::chrono::seconds(5));
        _coutMutex.lock();
        int i, n = tokenVector.size();
        std::cout << "Tokens: ";
        for (i = 0; i < n; ++i) std::cout << tokenVector[i] << (i == n - 1 ? "" : ", ");
        std::cout << std::endl;
        _coutMutex.unlock();*/
    }

    int MapServer::exitProcess(void)
    {
        if (MapData::destroyInstance() != 0)
        {
            std::cerr << "Error in QuizData::destroyInstance()" << std::endl;
            exit(-1);
        }

        exit(0);
    }
}
