 #include "SimpleAnswerCategory.h"
 #include "RandomNumberGenerator.h"
 #include "SimpleAnswerQuestion.h"
 #include "QuizData.h"
 #include "CompleteQuestion.h"

 namespace produce_questions
 {
    const CompleteQuestion *SimpleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);

        QuizData *quizData = QuizData::instance();
        const SimpleAnswerQuestion *question = quizData->getSimpleAnswerQuestion(_questionListId, draw, _proximityCriterionType);

        CompleteQuestion *completeQuestion = new CompleteQuestion(question->getQuestion(), SIMPLE, choiceCount);
        completeQuestion->addChoice(question->getAnswer(), question->getComment(), true);

        std::set<unsigned int> excludedValues;


        return 0;
    }

 }
