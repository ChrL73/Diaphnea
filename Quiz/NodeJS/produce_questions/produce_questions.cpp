#include "QuestionProducer.h"

#include <iostream>

int main(int argc, char *argv[])
{
    if (argc < 3)
    {
        std::cerr << "usage: produce_questions.exe levelId languageId" << std::endl;
        return -1;
    }

    produce_questions::QuestionProducer questionProducer(argv[1], argv[2]);
    int result = questionProducer.run();

    return result;
}
