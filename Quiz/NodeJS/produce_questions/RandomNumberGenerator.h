#pragma once

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

    public:
        static int verify(void);
        static unsigned int getRandomInt(int valueCount);

    };
}

