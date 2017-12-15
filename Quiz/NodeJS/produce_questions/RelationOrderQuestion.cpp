#include "RelationOrderQuestion.h"
#include "QuizData.h"
#include "CompleteQuestion.h"
#include "RandomNumberGenerator.h"
#include "CompleteQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
#include "RelationOrderChoice.h"

namespace produce_questions
{
    RelationOrderQuestion::RelationOrderQuestion(const std::string& question, int choiceCount, const std::string& choiceListId, const std::string& mapId) :
        _question(question), _choiceCount(choiceCount), _choiceListId(choiceListId), _mapId(mapId)
    {
        QuizData *quizData = QuizData::instance();
        int i;
        for (i = 0; i < _choiceCount; ++i)
        {
            const RelationOrderChoice *choice = quizData->getRelationOrderChoice(_choiceListId, i);
            _choiceVector.push_back(choice);
        }
    }

    CompleteQuestion *RelationOrderQuestion::getNewQuestion(int choiceCount, double distribParameter, const MapParameters *mapParameters) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(mapParameters, _question, produce_questions::SIMPLE, choiceCount);

        const MapSubParameters *questionParameters = mapParameters->getQuestionParameters();
        const MapSubParameters *answerParameters = mapParameters->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = mapParameters->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(_mapId, questionParameters);

        int draw1 = RandomNumberGenerator::getRandomInt(_choiceCount - choiceCount + 1);
        const RelationOrderChoice *answer = _choiceVector[draw1];
        completeQuestion->addChoice(answer->getText(), "", true);
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(answer->getMapId(), answerParameters);

        std::set<unsigned int> excludedValues;
        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            int draw2 = RandomNumberGenerator::getRandomInt(_choiceCount - draw1 - 1, distribParameter, excludedValues);
            excludedValues.insert(draw2);
            const RelationOrderChoice *wrongChoice = _choiceVector[draw1 + draw2 + 1];
            completeQuestion->addChoice(wrongChoice->getText(), "", false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}

