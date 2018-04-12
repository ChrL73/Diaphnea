#include "AttributeOrderCategory.h"
#include "CompleteQuestion.h"
#include "RandomNumberGenerator.h"
#include "MapParameters.h"
#include "MapSubParameters.h"

namespace produce_questions
{
    CompleteQuestion *AttributeOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), getQuestionText(), produce_questions::SIMPLE, choiceCount);

        int draw = RandomNumberGenerator::getRandomInt(_maxIndex + 1);
        completeQuestion->addChoice(getChoiceText(draw), getChoiceComment(draw), true);

        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(draw), answerParameters);

        int minIndex = getChoiceMinIndex(draw);
        int n = _choiceCount;
        std::set<unsigned int> excludedValues;

        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(n - minIndex, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice(getChoiceText(minIndex + draw), getChoiceComment(minIndex + draw), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(minIndex + draw), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
