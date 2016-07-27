#include "QuestionProducer.h"
#include "QuizData.h"
#include "Level.h"

namespace produce_questions
{
    int QuestionProducer::run(void)
    {
        QuizData *quizData = QuizData::instance();
        if (quizData == 0)
        {
            // Todo: handle error
            return -1;
        }

        Level *level = quizData->getLevel(_levelId);

        if (QuizData::destroyInstance() != 0)
        {
            // Todo: handle error
            return -1;
        }

        return 0;
    }
}
