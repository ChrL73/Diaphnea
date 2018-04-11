#include "QuestionProducer.h"

//#include <ctime>
//#include <iostream>

int main(void)
{
    //clock_t t0 = std::clock();

    produce_questions::QuestionProducer questionProducer;
    int result = questionProducer.run();

    //std::cout << std::endl << 0.001 * static_cast<double>(std::clock() - t0)  << "ms" << std::endl;

    return result;
}
