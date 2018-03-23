#include "QuestionProducer.h"

#include <iostream>

int main(int argc, char *argv[])
{
    if (argc < 5)
    {
        std::cerr << "usage: produce_questions.exe levelId languageId dbHost dbName [dbUser [dbPassword]]" << std::endl;
        return -1;
    }

    produce_questions::QuestionProducer questionProducer(argv[1], argv[2], argv[3], argv[4], argc < 6 ? "" : argv[5], argc < 7 ? "" : argv[6]);
    int result = questionProducer.run();

    return result;
}
