#include "RandomNumberGenerator.h"
#include <ctime>
#include <cstdlib>
#include <random>

namespace produce_questions
{
    bool RandomNumberGenerator::_initialized = false;
    unsigned int RandomNumberGenerator::_shift1[32];
    unsigned int RandomNumberGenerator::_shift2[32];
    int RandomNumberGenerator::_counter1 = 0;
    int RandomNumberGenerator::_counter2 = 0;

    void RandomNumberGenerator::initialize(void)
    {
        srand(static_cast<unsigned int>(time(0)));

        std::random_device rd;
        unsigned int n = rd();
        int i;
        for (i = 0; i < 32; ++i)
        {
            _shift1[i] = n;
            n = (n << 1) | (n >> 31);
        }

        int *p = new int;
        int q = 0;

        #if defined(__linux)
            long q0 = reinterpret_cast<long>(p);
            int q1 = static_cast<int>((q0 & 0xffffffff00000000) >> 32);
            int q2 = static_cast<int>(q0 & 0xffffffff);
            q = q1 + q2;
        #elif defined(_WIN32)
            q = reinterpret_cast<int>(p);
        #endif

        delete p;

        for (i = 0; i < 32; ++i)
        {
            _shift2[i] = q;
            q = (q << 1) | (q >> 31);
        }

        _initialized = true;
    }

    int RandomNumberGenerator::verify(void)
    {
        if (sizeof(unsigned int) != 4) return -1;

        if (std::random_device::min() != 0 || std::random_device::max() != 0xFFFFFFFF) return -1;

        #if defined(__linux) && !defined(_WIN32)
            if (sizeof(int *) != 8) return -1;
            if (sizeof(long) != 8) return -1;
            if (RAND_MAX != 0x7FFFFFFF) return -1;
            if (CLOCKS_PER_SEC != 1000000) return -1;
        #elif defined(_WIN32)
            if (sizeof(int *) != 4) return -1;
            if (RAND_MAX != 0x7FFF) return -1;
            if (CLOCKS_PER_SEC != 1000) return -1;
        #else
            #error Unknown platform
        #endif

        if (!_initialized) initialize();
        return 0;
    }

    unsigned int RandomNumberGenerator::shift1(void)
    {
        ++_counter1;
        if (_counter1 == 32) _counter1 = 0;
        return _shift1[_counter1];
    }

    unsigned int RandomNumberGenerator::shift2(void)
    {
        ++_counter2;
        if (_counter2 == 32) _counter2 = 0;
        return _shift2[_counter2];
    }

    unsigned int RandomNumberGenerator::getRandomInt(void)
    {
        if (!_initialized) initialize();
        return static_cast<unsigned int>(rand()) + shift1() + shift2() + static_cast<unsigned int>(std::clock());
    }

    double RandomNumberGenerator::getRandomDouble(void)
    {
        return static_cast<double>(getRandomInt()) / static_cast<double>(0xFFFFFFFF);
    }

    unsigned int RandomNumberGenerator::getRandomInt(int valueCount)
    {
        return getRandomInt() % valueCount;
    }

    double RandomNumberGenerator::inverseCumulativeDistribution(double distribParameter, double y)
    {
        if (distribParameter == 0.5) return y;
        if (distribParameter < 0.0) distribParameter = 0.0;
        if (distribParameter > 1.0) distribParameter = 1.0;

        const double a = -5.0;
        const double b = 1.99;

        double u = b * (distribParameter - 0.5);
        double alpha = a * log ((1.0 + u) / (1.0 - u));
        return -log (1.0 - y * (1 -exp (-alpha))) / alpha;
    }

    unsigned int RandomNumberGenerator::getRandomInt(int valueCount, const std::set<unsigned int>& excludedValues)
    {
        valueCount -= excludedValues.size();
        if (valueCount <= 0) return 0;

        unsigned int draw = getRandomInt(valueCount);

        std::set<unsigned int>::const_iterator it = excludedValues.begin();
        for (; it != excludedValues.end(); ++it)
        {
            if (draw < *it) break;
            ++draw;
        }

        return draw;
    }

    unsigned int RandomNumberGenerator::getRandomInt(int valueCount, double distribParameter)
    {
        double draw = getRandomDouble();
        double x = inverseCumulativeDistribution(distribParameter, draw);
        int i = static_cast<int>(x * static_cast<double>(valueCount));
        if (i == valueCount) i = 0;
        return i;
    }

    unsigned int RandomNumberGenerator::getRandomInt(int valueCount, double distribParameter, std::set<unsigned int>& excludedValues)
    {
        valueCount -= excludedValues.size();
        if (valueCount < 1) return 0;

        unsigned int draw = getRandomInt(valueCount, distribParameter);

        std::set<unsigned int>::const_iterator it = excludedValues.begin();
        for (; it!=excludedValues.end(); ++it)
        {
            if (draw < *it) break;
            ++draw;
        }

        return draw;
    }
}
