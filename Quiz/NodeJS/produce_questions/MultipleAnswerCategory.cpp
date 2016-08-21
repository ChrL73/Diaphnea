#include "MultipleAnswerCategory.h"
#include "QuizData.h"

namespace produce_questions
{
    MultipleAnswerCategory::MultipleAnswerCategory(unsigned int weightIndex, int questionCount, const std::string& questionListId, int choiceCount, const std::string& choiceListId,
                                                   double _distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType) :
        Category(weightIndex), _questionCount(questionCount), _questionListId(questionListId), _choiceCount(choiceCount), _choiceListId(choiceListId),
        _distribParameterCorrection(_distribParameterCorrection), _proximityCriterionType(proximityCriterionType)
    {
        QuizData *quizData = QuizData::instance();
        int i;
        for (i = 0; i < _choiceCount; ++i)
        {
            const Choice *choice = quizData->getChoice(_choiceListId, i, _proximityCriterionType);
            _choiceVector.push_back(choice);
        }
    }

    CompleteQuestion *MultipleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        return 0;
    }
}
