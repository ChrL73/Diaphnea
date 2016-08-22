#include "RandomNumberGenerator.h"
#include <ctime>
#include <cstdlib>
#include <random>

#if RAND_MAX != 0x7FFFFFFF
    #ifdef _WIN32
        #if RAND_MAX != 0x7FFF  
            #error unexpected RAND_MAX value
        #else
            #pragma message("RandomNumberGenerator target is Linux. It will not behave as expected on Windows. Windows is used for development purpose")
        #endif
    #else
        #error unexpected RAND_MAX value
    #endif
#endif

#if CLOCKS_PER_SEC != 1000000
    #ifdef _WIN32
        #if CLOCKS_PER_SEC != 1000
            #error unexpected RAND_MAX value
        #else
            #pragma message("RandomNumberGenerator target is Linux. It will not behave as expected on Windows. Windows is used for development purpose")
        #endif
    #else
        #error unexpected CLOCKS_PER_SEC value
    #endif
#endif

namespace produce_questions
{
    bool RandomNumberGenerator::_initialized = false;
    unsigned int RandomNumberGenerator::_shift[32];
    int RandomNumberGenerator::_counter = 0;

    void RandomNumberGenerator::initialize(void)
    {
        srand(static_cast<unsigned int>(time(0)));

        std::random_device rd;
        unsigned int n = rd();
        int i;
        for (i = 0; i < 32; ++i)
        {
            _shift[i] = n;
            n = (n << 1) | (n >> 31);
        }

        _initialized = true;
    }

    int RandomNumberGenerator::verify(void)
    {
        if (sizeof(unsigned int) != 4) return -1;
        if (std::random_device::min() != 0 || std::random_device::max() != 0xFFFFFFFF) return -1;
        if (!_initialized) initialize();
        return 0;
    }

    unsigned int RandomNumberGenerator::shift(void)
    {
        ++_counter;
        if (_counter == 32) _counter = 0;
        return _shift[_counter];
    }

    unsigned int RandomNumberGenerator::getRandomInt(void)
    {
        if (!_initialized) initialize();
        return static_cast<unsigned int>(rand()) + shift() + static_cast<unsigned int>(std::clock());
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
