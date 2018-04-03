#include "QuestionProducer.h"

int main(void)
{
    produce_questions::QuestionProducer questionProducer;
    int result = questionProducer.run();

    return result;
}
