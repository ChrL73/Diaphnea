#include <iostream>
#include <string>
#include <set>

#include <thread>
#include <future>
#include <mutex>

class ThreadInfo
{
private:
    std::thread *_t;
    std::future<void> *_f;
    std::string _str;

public:
    ThreadInfo(std::thread *t, std::future<void> *f, const std::string& str) :
        _t(t), _f(f), _str(str) {}
    ~ThreadInfo() { delete _t; }

    std::thread *getT(void) const { return _t; }
    const std::future<void> *getF(void) const { return _f; }
    const std::string& getStr(void) const { return _str; }
};

std::mutex mutex;
std::set<ThreadInfo *> threadSet;
bool stop = false;

void f(std::string str)
{
    std::cout << "Start: " << str << std::endl;
    std::this_thread::sleep_for(std::chrono::seconds(5));
    std::cout << "End: " << str << std::endl;
}

void deleteFunction(void)
{
    while(true)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(5));

        mutex.lock();

		std::vector<std::set<ThreadInfo *>::iterator> threadsToDelete;
        std::set<ThreadInfo *>::iterator it = threadSet.begin();
        for (; it != threadSet.end(); ++it)
        {
            std::future_status status = (*it)->getF()->wait_for(std::chrono::milliseconds(0));
            if (status == std::future_status::ready)
            {
                (*it)->getT()->join();
				threadsToDelete.push_back(it);
            }
        }

		int i, n = threadsToDelete.size();
		for (i = 0; i < n; ++i)
		{
			std::set<ThreadInfo *>::iterator it = threadsToDelete[i];
			std::cout << "Delete: " << (*it)->getStr() << std::endl;
			delete (*it);
			threadSet.erase(it);
		}

        if (stop && threadSet.empty())
        {
            mutex.unlock();
            break;
        }

        mutex.unlock();
    }
}

int main()
{
    std::thread deleteThread(deleteFunction);

    while(true)
    {
        std::string str;
        std::cin >> str;
        if (str[0] == 'q') break;

        std::packaged_task<void(std::string)> task(f);
        std::future<void> *future = new std::future<void>(task.get_future());
        std::thread *t = new std::thread(std::move(task), str);

        ThreadInfo *threadInfo  = new ThreadInfo(t, future, str);

        mutex.lock();
        threadSet.insert(threadInfo);
        mutex.unlock();
    }

    mutex.lock();
    stop = true;
    mutex.unlock();

    std::cout << "Waiting for threads..." << std::endl;
    deleteThread.join();

    return 0;
}
