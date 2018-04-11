#include "RelationOrderQuestion.h"
#include "CompleteQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    CompleteQuestion *RelationOrderQuestion::getNewQuestion(int choiceCount, double distribParameter, const MapParameters *mapParameters) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(mapParameters, getQuestion(), produce_questions::SIMPLE, choiceCount);

        const MapSubParameters *questionParameters = mapParameters->getQuestionParameters();
        const MapSubParameters *answerParameters = mapParameters->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = mapParameters->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(getMapId(), questionParameters);

        int draw1 = RandomNumberGenerator::getRandomInt(_choiceCount - choiceCount + 1);
        completeQuestion->addChoice(getChoiceText(draw1), "", true);
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(draw1), answerParameters);

        std::set<unsigned int> excludedValues;
        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            int draw2 = RandomNumberGenerator::getRandomInt(_choiceCount - draw1 - 1, distribParameter, excludedValues);
            excludedValues.insert(draw2);
            completeQuestion->addChoice(getChoiceText(draw1 + draw2 + 1), "", false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(draw1 + draw2 + 1), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
