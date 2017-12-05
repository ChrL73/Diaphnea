#include "MultipleAnswerCategory.h"
#include "QuizData.h"
#include "RandomNumberGenerator.h"
#include "MultipleAnswerQuestion.h"
#include "CompleteQuestion.h"
#include "TextAndComment.h"
#include "Choice.h"
#include "MapParameters.h"
 #include "MapSubParameters.h"

namespace produce_questions
{
    MultipleAnswerCategory::MultipleAnswerCategory(unsigned int weightIndex, const MapParameters *mapParameters, int questionCount, const std::string& questionListId,
                                                   int choiceCount, const std::string& choiceListId, double distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType) :
        Category(weightIndex, mapParameters), _questionCount(questionCount), _questionListId(questionListId), _choiceCount(choiceCount), _choiceListId(choiceListId),
        _distribParameterCorrection(distribParameterCorrection), _proximityCriterionType(proximityCriterionType)
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

        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), question->getQuestion(), produce_questions::MULTIPLE, choiceCount);

        const MapSubParameters *questionParameters = getMapParameters()->getQuestionParameters();
        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getQuestionMapId(), questionParameters);

        int valueCount = choiceCount - 1;
        if (valueCount > static_cast<int>(question->getAnswerCount())) valueCount = question->getAnswerCount();
        int rightChoiceCount = 1 + RandomNumberGenerator::getRandomInt(valueCount);

        int wrongChoiceCount = question->getWrongChoiceCount();
        if (rightChoiceCount + wrongChoiceCount < choiceCount) rightChoiceCount = choiceCount - wrongChoiceCount;

        int i, n;
        if (getMapParameters()->getAllAnswersSelectionMode() && answerParameters->getDrawDepth() != 0)
        {
            n = question->getAnswerCount();
            for (i = 0; i < n; ++i) completeQuestion->addMapId(question->getAnswerMapId(i), answerParameters);
        }

        std::set<unsigned int> excludedValues;
        for (i = 0; i < rightChoiceCount; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(question->getAnswerCount(), excludedValues);
            excludedValues.insert(draw);
            const TextAndComment *textAndComment = question->getAnswer(draw);
            completeQuestion->addChoice(textAndComment->getText(), textAndComment->getComment(), true);
            if (!getMapParameters()->getAllAnswersSelectionMode() && answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getAnswerMapId(draw), answerParameters);
        }

        excludedValues.clear();
        n = choiceCount - rightChoiceCount;
        for (i = 0; i < n; ++i)
        {
            if (_proximityCriterionType != produce_questions::POINT_3D) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            const Choice *wrongChoice = question->getWrongChoice(draw);
            completeQuestion->addChoice(wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
