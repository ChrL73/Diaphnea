#include "Thread2.h"

#include <iostream>
#include <set>
#include <vector>

#include <thread>
#include <future>
#include <mutex>

class ThreadInfo
{
private:
    std::thread _t;
    std::future<void> _f;

public:
    ThreadInfo(std::thread&& t, std::future<void>&& f) :
        _t(std::move(t)), _f(std::move(f)) {}

    std::thread& getT(void) { return _t; }
    const std::future<void>& getF(void) const { return _f; }
};

std::mutex mutex;
std::set<ThreadInfo *> threadSet;
bool stop = false;

void deleteFunction(void)
{
    std::vector<std::set<ThreadInfo *>::iterator> threadsToDelete;

    while(true)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(10));

        mutex.lock();

        std::set<ThreadInfo *>::iterator it = threadSet.begin();
        for (; it != threadSet.end(); ++it)
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
            threadSet.erase(it);
        }

        threadsToDelete.clear();

        if (stop && threadSet.empty())
        {
            mutex.unlock();
            break;
        }

        mutex.unlock();
    }
}

void f2()
{
    std::this_thread::sleep_for(std::chrono::milliseconds(100));
}

void Thread2::run(void)
{
    std::thread deleteThread(deleteFunction);

    int i;
    for (i = 0; i < 100000; ++i)
    {
        std::cout << i << " ";

        std::packaged_task<void()> task(f2);
        std::future<void> future = task.get_future();
        std::thread t(std::move(task));

        ThreadInfo *threadInfo = new ThreadInfo(std::move(t), std::move(future));

        mutex.lock();
        threadSet.insert(threadInfo);
        mutex.unlock();
    }

    mutex.lock();
    stop = true;
    mutex.unlock();

    std::cout << "Waiting for threads..." << std::endl;
    deleteThread.join();
}

