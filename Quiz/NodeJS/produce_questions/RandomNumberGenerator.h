#pragma once

#include <set>

namespace produce_questions
{
    class RandomNumberGenerator
    {
    private:
        static bool _initialized;
        static void initialize(void);
        static unsigned int _shift[32];
        static int _counter;

        static unsigned int shift(void);
        static unsigned int getRandomInt(void);
        static double getRandomDouble (void);
        static double inverseCumulativeDistribution (double distribParameter, double y);

    public:
        static int verify(void);
        static unsigned int getRandomInt(int valueCount);
        static unsigned int getRandomInt(int valueCount, const std::set<unsigned int>& excludedValues);
        static unsigned int getRandomInt (int valueCount, double distribParameter);
        static unsigned int getRandomInt (int valueCount, double distribParameter, std::set<unsigned int>& excludedValues);
    };
}

