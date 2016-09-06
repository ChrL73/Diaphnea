#include "AttributeOrderCategory.h"
#include "AttributeOrderChoice.h"
#include "QuizData.h"
#include "CompleteQuestion.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    AttributeOrderCategory::AttributeOrderCategory(unsigned int weightIndex, const std::string& questionText, int choiceCount, const std::string& choiceListId, double distribParameterCorrection, int maxIndex) :
        Category(weightIndex), _questionText(questionText), _choiceCount(choiceCount), _choiceListId(choiceListId), _distribParameterCorrection(distribParameterCorrection), _maxIndex(maxIndex)
    {
        QuizData *quizData = QuizData::instance();
        int i;
        for (i = 0; i < _choiceCount; ++i)
        {
            const AttributeOrderChoice *choice = quizData->getAttributeOrderChoice(_choiceListId, i);
            _choiceVector.push_back(choice);
        }
    }

    CompleteQuestion *AttributeOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(_questionText, produce_questions::SIMPLE, choiceCount);

        int draw = RandomNumberGenerator::getRandomInt(_maxIndex + 1);
        const AttributeOrderChoice *choice = _choiceVector[draw];
        completeQuestion->addChoice(choice->getChoiceText(), choice->getComment(), true);

        int minIndex = choice->getMinIndex();
        int n = _choiceVector.size();
        std::set<unsigned int> excludedValues;

        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(n - minIndex, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            const AttributeOrderChoice *choice = _choiceVector[minIndex + draw];
            completeQuestion->addChoice(choice->getChoiceText(), choice->getComment(), false);
        }

        return completeQuestion;
    }
}
