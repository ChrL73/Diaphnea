 #include "SimpleAnswerCategory.h"
 #include "RandomNumberGenerator.h"
 #include "SimpleAnswerQuestion.h"
 #include "QuizData.h"
 #include "CompleteQuestion.h"
 #include "Choice.h"
 #include "MapParameters.h"
 #include "MapSubParameters.h"

 namespace produce_questions
 {
    SimpleAnswerCategory::SimpleAnswerCategory(unsigned int weightIndex, const MapParameters *mapParameters, int questionCount, const std::string& questionListId, int choiceCount,
                                               const std::string& choiceListId, double distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType) :
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

    CompleteQuestion *SimpleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        QuizData *quizData = QuizData::instance();
        const SimpleAnswerQuestion *question = quizData->getSimpleAnswerQuestion(_questionListId, draw, _proximityCriterionType, _choiceVector);

        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), question->getQuestion(), produce_questions::SIMPLE, choiceCount);
        completeQuestion->addChoice(question->getAnswer(), question->getComment(), true);

        const MapSubParameters *questionParameters = getMapParameters()->getQuestionParameters();
        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getQuestionMapId(), questionParameters);
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getAnswerMapId(), answerParameters);

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
            const Choice *wrongChoice = question->getWrongChoice1(draw);
            completeQuestion->addChoice(wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        excludedValues.clear();
        for (i = 0; i < n2; ++i)
        {
            if (_proximityCriterionType == produce_questions::NONE) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            const Choice *wrongChoice = question->getWrongChoice2(draw);
            completeQuestion->addChoice (wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        return completeQuestion;
    }

 }
