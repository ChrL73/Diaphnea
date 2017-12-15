#include "RelationOrderCategory.h"
#include "RandomNumberGenerator.h"
#include "RelationOrderQuestion.h"
#include "QuizData.h"

namespace produce_questions
{
    CompleteQuestion *RelationOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        QuizData *quizData = QuizData::instance();
        const RelationOrderQuestion *question = quizData->getRelationOrderQuestion(_questionListId, draw);

        CompleteQuestion *completeQuestion = question->getNewQuestion(choiceCount, distribParameter + _distribParameterCorrection, getMapParameters());

        return completeQuestion;
    }
}
