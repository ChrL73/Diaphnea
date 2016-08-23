#include "QuestionProducer.h"

int main(int argc, char *argv[])
{
    if (argc < 3) return -1;

    produce_questions::QuestionProducer questionProducer(argv[1], argv[2]);
    int result = questionProducer.run();

    return result;
}
