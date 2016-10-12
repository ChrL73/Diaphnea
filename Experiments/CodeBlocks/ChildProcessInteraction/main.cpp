#include <iostream>
#include <string>
#include <set>
#include <vector>
#include <ctime>

#include <thread>
#include <future>
#include <mutex>

class ThreadInfo
{
private:
    std::thread _t;
    std::future<void> _f;
    std::string _str;

public:
    ThreadInfo(std::thread&& t, std::future<void>&& f, const std::string&& str) :
        _t(std::move(t)), _f(std::move(f)), _str(std::move(str)) {}

    std::thread& getT(void) { return _t; }
    const std::future<void>& getF(void) const { return _f; }
    const std::string& getStr(void) const { return _str; }
};

std::set<ThreadInfo *> threadSet;
std::mutex threadSetMutex;

time_t lastEntryTime;
std::mutex timeMutex;

std::mutex coutMutex;

void f(std::string str)
{
    coutMutex.lock();
    std::cout << "Start: " << str << std::endl;
    coutMutex.unlock();

    std::this_thread::sleep_for(std::chrono::seconds(5));

    coutMutex.lock();
    std::cout << "End: " << str << std::endl;
    coutMutex.unlock();
}

void deleteFunction(void)
{
    std::vector<std::set<ThreadInfo *>::iterator> threadsToDelete;

    while(true)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(5));

        threadSetMutex.lock();

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

            coutMutex.lock();
			std::cout << "Delete: " << (*it)->getStr() << std::endl;
			coutMutex.unlock();

			delete (*it);
			threadSet.erase(it);
		}

		threadsToDelete.clear();

        threadSetMutex.unlock();
    }
}

void exitProcess()
{
    // We don't try to properly stop threads and wait for them.
    // We roughly interrupt all threads with 'exit'
    exit(0);
}

void timeoutFunction(void)
{
    while (true)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(200));

        timeMutex.lock();
        if (time(0) - lastEntryTime > 10)
        {
            timeMutex.unlock();
            std::cout << "Timeout reached, exit process..." << std::endl;
            exitProcess();
        }
        timeMutex.unlock();
    }
}

int main()
{
    std::thread deleteThread(deleteFunction);
    std::thread timeoutThread(timeoutFunction);
    lastEntryTime = time(0);

    while(true)
    {
        std::string str;
        std::cin >> str;

        // When the process was spawned by node and node crashes, 'std::cin >> str' returns and str size is 0
        if (str.size() == 0 || str[0] == 'q') break;

        timeMutex.lock();
        lastEntryTime = time(0);
        timeMutex.unlock();

        std::packaged_task<void(std::string)> task(f);
        std::future<void> future = task.get_future();
        std::thread t(std::move(task), str);

        ThreadInfo *threadInfo = new ThreadInfo(std::move(t), std::move(future), std::move(str));

        threadSetMutex.lock();
        threadSet.insert(threadInfo);
        threadSetMutex.unlock();
    }

    std::cout << "Stop requested, exit process..." << std::endl;
    exitProcess();
}
