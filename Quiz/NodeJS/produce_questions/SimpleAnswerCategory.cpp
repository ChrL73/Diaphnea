 #include "SimpleAnswerCategory.h"
 #include "RandomNumberGenerator.h"
 #include "SimpleAnswerQuestion.h"
 #include "QuizData.h"

 namespace produce_questions
 {
    const CompleteQuestion *SimpleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);

        QuizData *quizData = QuizData::instance();
        SimpleAnswerQuestion *question = quizData->getSimpleAnswerQuestion(_questionListId, draw);

        return 0;
    }

 }
