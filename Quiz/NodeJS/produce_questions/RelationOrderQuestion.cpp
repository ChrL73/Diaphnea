#include "RelationOrderQuestion.h"
#include "QuizData.h"
#include "CompleteQuestion.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    RelationOrderQuestion::RelationOrderQuestion(const std::string& question, int choiceCount, const std::string& choiceListId) :
        _question(question), _choiceCount(choiceCount), _choiceListId(choiceListId)
    {
        QuizData *quizData = QuizData::instance();
        int i;
        for (i = 0; i < _choiceCount; ++i)
        {
            std::string choice = quizData->getRelationOrderChoice(_choiceListId, i);
            _choiceVector.push_back(choice);
        }
    }

    CompleteQuestion *RelationOrderQuestion::getNewQuestion(int choiceCount, double distribParameter) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(_question, produce_questions::SIMPLE, choiceCount);

        int draw1 = RandomNumberGenerator::getRandomInt(_choiceCount - choiceCount + 1);
        std::string answer = _choiceVector[draw1];
        completeQuestion->addChoice(answer, "", true);

        std::set<unsigned int> excludedValues;
        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            int draw2 = RandomNumberGenerator::getRandomInt(_choiceCount - draw1 - 1, distribParameter, excludedValues);
            excludedValues.insert(draw2);
            completeQuestion->addChoice(_choiceVector[draw1 + draw2 + 1], "", false);
        }

        return completeQuestion;
    }
}

