#include "Thread1.h"

#include <iostream>
#include <thread>

// With this code, the 'thread' object is not deleted when a thread terminates ->
// - Memory leaks
// - 'core dumped' when i==32747

void f1(void)
{
    std::this_thread::sleep_for(std::chrono::milliseconds(100));
}

void Thread1::run(void)
{
    int i;
    for (i = 0; i < 100000; ++i)
    {
        std::cout << i << " ";
        std::thread *thread = new std::thread(f1);
    }
}


