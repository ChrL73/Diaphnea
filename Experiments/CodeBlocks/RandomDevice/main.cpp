#include <iostream>
#include <random>
#include <ctime>
#include <chrono>

#if CLOCKS_PER_SEC != 1000000
#error unexpected CLOCKS_PER_SEC value
#endif

int main(void)
{
    std::cout << "min: " << std::random_device::min() << std::endl;
    std::cout << "max: " << std::random_device::max() << std::endl;

    std::random_device rd;
    std::cout << "draw: "  << rd() << std::endl;
    std::cout << "entropy: "  << rd.entropy() << std::endl; // With g++, 'entropy' returns always 0

    /*int i;
    for (i = 0; i < 1000; ++i)
    {
        std::cout << rd() << " ";
    }*/

    std::cout << "time: " << time(0) << std::endl;
    long c1 = std::clock(), c2 = clock();
    std::cout << "c1: " << c1 << std::endl;
    std::cout << "c2: " << c2 << std::endl;
    std::cout << "c2 - c1: " << c2 - c1 << std::endl;
    std::cout << "CLOCKS_PER_SEC: " << CLOCKS_PER_SEC << std::endl;
    std::cout << std::endl;

    long t1 = std::chrono::system_clock::now().time_since_epoch().count();
    long t2 = std::chrono::system_clock::now().time_since_epoch().count();
    std::cout << "t1: " << t1 << std::endl;
    std::cout << "t2: " << t2 << std::endl;
    std::cout << "t2 - t1: " << t2 - t1 << std::endl;
    std::cout << std::endl;

    srand(time(0));
    std::cout << "RAND_MAX: " << RAND_MAX << std::endl;
    std::cout << "rand: " << rand() << std::endl;

    return 0;
}
