#include "RelationOrderCategory.h"
#include "RandomNumberGenerator.h"
#include "RelationOrderQuestion.h"

namespace produce_questions
{
    CompleteQuestion *RelationOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        const RelationOrderQuestion *question = getQuestion(draw);

        CompleteQuestion *completeQuestion = question->getNewQuestion(choiceCount, distribParameter + _distribParameterCorrection, getMapParameters());

        return completeQuestion;
    }
}
