 #include "SimpleAnswerCategory.h"
 #include "RandomNumberGenerator.h"
 #include "SimpleAnswerQuestion.h"
 #include "QuizData.h"
 #include "CompleteQuestion.h"
 #include "Choice.h"

 namespace produce_questions
 {
    SimpleAnswerCategory::SimpleAnswerCategory(unsigned int weightIndex, int questionCount, const std::string& questionListId, int choiceCount, const std::string& choiceListId,
                                               double distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType) :
        Category(weightIndex), _questionCount(questionCount), _questionListId(questionListId), _choiceCount(choiceCount), _choiceListId(choiceListId),
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

    CompleteQuestion *SimpleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        QuizData *quizData = QuizData::instance();
        const SimpleAnswerQuestion *question = quizData->getSimpleAnswerQuestion(_questionListId, draw, _proximityCriterionType, _choiceVector);

        CompleteQuestion *completeQuestion = new CompleteQuestion(question->getQuestion(), produce_questions::SIMPLE, choiceCount);
        completeQuestion->addChoice(question->getAnswer(), question->getComment(), true);

        std::set<unsigned int> excludedValues;
        int wrongChoiceCount1 = question->getWrongChoiceCount1();
        int wrongChoiceCount2 = question->getWrongChoiceCount2();

        int n1 = RandomNumberGenerator::getRandomInt (choiceCount - 1);
        if (n1 > wrongChoiceCount1) n1 = wrongChoiceCount1;
        int n2 = choiceCount - 1 - n1;
        if (n2 > wrongChoiceCount2)
        {
            n2 = wrongChoiceCount2;
            n1 = choiceCount - 1 - n2;
        }

        int i;
        for (i = 0; i < n1; ++i)
        {
            if (_proximityCriterionType == produce_questions::NONE) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount1, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount1, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice(question->getWrongChoice1(draw)->getChoiceText(), question->getWrongChoice1(draw)->getComment(), false);
        }

        excludedValues.clear();
        for (i = 0; i < n2; ++i)
        {
            if (_proximityCriterionType == produce_questions::NONE) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice (question->getWrongChoice2(draw)->getChoiceText(), question->getWrongChoice2(draw)->getComment(), false);
        }

        return completeQuestion;
    }

 }
