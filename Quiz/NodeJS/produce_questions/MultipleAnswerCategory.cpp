#include "MultipleAnswerCategory.h"
#include "QuizData.h"
#include "RandomNumberGenerator.h"
#include "MultipleAnswerQuestion.h"
#include "CompleteQuestion.h"
#include "TextAndComment.h"
#include "Choice.h"

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
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        QuizData *quizData = QuizData::instance();
        const MultipleAnswerQuestion *question = quizData->getMultipleAnswerQuestion(_questionListId, draw, _proximityCriterionType, _choiceVector);

        CompleteQuestion *completeQuestion = new CompleteQuestion(question->getQuestion(), produce_questions::MULTIPLE, choiceCount);

        int valueCount = choiceCount - 1;
        if (valueCount > static_cast<int>(question->getAnswerCount())) valueCount = question->getAnswerCount();
        int rightChoiceCount = 1 + RandomNumberGenerator::getRandomInt(valueCount);

        int wrongChoiceCount = question->getWrongChoiceCount();
        if (rightChoiceCount + wrongChoiceCount < choiceCount) rightChoiceCount = choiceCount - wrongChoiceCount;

        std::set<unsigned int> excludedValues;
        int i;
        for (i = 0; i < rightChoiceCount; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(question->getAnswerCount(), excludedValues);
            excludedValues.insert(draw);
            const TextAndComment *textAndComment = question->getAnswer(draw);
            completeQuestion->addChoice(textAndComment->getText(), textAndComment->getComment(), true);
        }

        excludedValues.clear();
        int n = choiceCount - rightChoiceCount;
        for (i = 0; i<n; ++i)
        {
            if (_proximityCriterionType != produce_questions::POINT_3D) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice(question->getWrongChoice(draw)->getChoiceText(), question->getWrongChoice(draw)->getComment(), false);
        }

        return completeQuestion;
    }
}