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

std::mutex mutex;
std::set<ThreadInfo *> threadSet;
bool stop = false;
time_t lastEntryTime;

void f(std::string str)
{
    std::cout << "Start: " << str << std::endl;
    std::this_thread::sleep_for(std::chrono::seconds(5));
    std::cout << "End: " << str << std::endl;
}

void deleteFunction(void)
{
    std::vector<std::set<ThreadInfo *>::iterator> threadsToDelete;
    bool forceExit = false;

    while(true)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(5));

        mutex.lock();

        if (time(0) - lastEntryTime > 10)
        {
            stop = true;
            forceExit = true;
        }

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
			std::cout << "Delete: " << (*it)->getStr() << std::endl;
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

    if (forceExit)
    {
        std::cout << "Timeout reached, force 'std::cin' interruption with 'exit()'..." << std::endl;
        exit(0);
    }
}

int main()
{
    std::thread deleteThread(deleteFunction);
    lastEntryTime = time(0);

    while(true)
    {
        std::string str;
        std::cin >> str;
        if (str[0] == 'q') break;

        mutex.lock();
        lastEntryTime = time(0);
        if (stop)
        {
            mutex.unlock();
            break;
        }
        mutex.unlock();

        std::packaged_task<void(std::string)> task(f);
        std::future<void> future = task.get_future();
        std::thread t(std::move(task), str);

        ThreadInfo *threadInfo = new ThreadInfo(std::move(t), std::move(future), std::move(str));

        mutex.lock();
        threadSet.insert(threadInfo);
        mutex.unlock();
    }

    mutex.lock();
    stop = true;
    mutex.unlock();

    std::cout << "Waiting for threads..." << std::endl;
    deleteThread.join();
    std::cout << "All threads terminated..." << std::endl;

    return 0;
}
