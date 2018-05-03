#include "MapServer.h"
#include "ThreadInfo.h"
#include "Request.h"
#include "MessageTypeEnum.h"
#include "ErrorEnum.h"
#include "CommonData.h"

#include <iostream>
#include <vector>
#include <cstring>

#include <thread>
#include <future>

namespace map_server
{
    MapServer::MapServer(time_t timeoutInSeconds, int cleanThreadSleepMs, int checkTimeoutSleepMs, bool softExit) :
            _commonData(new CommonData()), _timeoutInSeconds(timeoutInSeconds), _cleanThreadSleepMs(cleanThreadSleepMs),
            _checkTimeoutSleepMs(checkTimeoutSleepMs), _softExit(softExit)
    {
    }

    MapServer::~MapServer()
    {
        delete _commonData;
    }

    int MapServer::run(void)
    {
        Request::setCoutMutex(&_coutMutex);
        //TextDisplayer::setCoutMutex(&_coutMutex);
        //SvgCreator::setCoutMutex(&_coutMutex);

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

        //TextDisplayer::clearClientMap();

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

            // tmp
            //request = "0 0 6 _France fr 800 600 0 n n n 01";

            // 2 expected responses:
            // 0 0 6 {"items":[[14,17,1],[624,16,1],[541,16,1],[626,16,1],[540,16,1],[627,16,1],[485,16,1],[625,16,1]],"xFocus":212.02,"yFocus":91.2119,"scale":5.2446}
            // 0 0 7 {"t":[["Ain (01)",192.887,92.3181]],"e":"01","x1":192.696,"x2":207.187,"y1":89.458,"y2":93.0808,"s":21,"look":15}

            //request = "0 0 6 _France fr 800 600 0 n n n Paris";

            // 2 expected responses:
            //0 0 6 {"items":[[401,64]],"xFocus":-24.8653,"yFocus":-215.634,"scale":28.5714}
            //0 0 7 {"t":[["Paris",-24.4359,-215.354]],"e":"Paris","x1":-24.4009,"x2":-22.6859,"y1":-215.914,"y2":-215.354,"s":22,"look":63}

            //request = "0 0 6 _France fr 800 600 0 n n n Seine";

            // 2 expected responses:
            // 0 0 6 {"items":[[795,55,3],[796,55,3],[797,55,3]],"xFocus":-8.7913,"yFocus":-178.268,"scale":2.16204}
            // 0 0 7 {"t":[["Seine",-2.10266,-179.549]],"e":"Seine","x1":-2.10266,"x2":17.3234,"y1":-185.1,"y2":-179.549,"s":17,"look":54}

            request = "0 0 6 _France fr 800 600 0 n n n Bretagne 22";

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
        try
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

            Request *request = Request::createRequest(_commonData, tokenVector);

            if (request != 0) request->execute();

            delete[] req;
            delete request;
        }
        catch (...)
        {
            _coutMutex.lock();
            std::cout << "0 -1 " << map_server::ERROR_ << " {\"error\":" << map_server::UNHANDLED_EXCEPTION
                << ",\"message\":\"Unhandled exception while processing this request: " << requestString << "\"}" << std::endl;
            _coutMutex.unlock();
            exit(0);
        }
    }
}
