#include "QuestionProducer.h"
#include "QuizData.h"
#include "Level.h"
#include "RandomNumberGenerator.h"
#include "Category.h"
#include "CompleteQuestion.h"

namespace produce_questions
{
    int QuestionProducer::run(void)
    {
        if (RandomNumberGenerator::verify() != 0)
        {
            std::cerr << "Error in RandomNumberGenerator::verify()" << std::endl;
            return -1;
        }

        QuizData *quizData = QuizData::instance(_dbHost, _dbName, _dbUser, _dbPassword);
        if (quizData == 0)
        {
            std::cerr << "Error in QuizData::instance()" << std::endl;
            return -1;
        }

        quizData->setLanguageId(_languageId);

        const Level *level = quizData->getLevel(_levelId);
        if (level == 0)
        {
            std::cerr << "Error in quizData->getLevel(_levelId)" << std::endl;
            QuizData::destroyInstance();
            return -1;
        }

        std::string json = "[";

        int i, questionCount = level->getQuestionCount();
        for (i = 0; i < questionCount; ++i)
        {
            Category *category = 0;
            int j, categoryCount = level->getCategoryCount();

            while (category == 0 || !category->isSuitable())
            {
                unsigned int draw = RandomNumberGenerator::getRandomInt(level->getWeightSum());

                for (j = 0; j < categoryCount; ++j)
                {
                    Category *jCategory = level->getCategories(j);
                    if (draw < jCategory->getWeightIndex())
                    {
                        category = jCategory;
                        break;
                    }
                }
            }

            int n = categoryCount / 10;
            if (n > 3) n = 3;
            category->reduceSuitability(n);
            for (j = 0; j < categoryCount; ++j)
            {
                if (category != level->getCategories(j)) level->getCategories(j)->increaseSuitability();
            }

            CompleteQuestion *question = category->getNewQuestion(level->getChoiceCount(), level->getDistribParameter());

            json += question->getJson();
            if (i != questionCount - 1) json += ",";

            delete question;
        }

        json += "]";

        if (QuizData::destroyInstance() != 0)
        {
            std::cerr << "Error in QuizData::destroyInstance()" << std::endl;
            return -1;
        }

        std::cout << json;

        return 0;
    }
}
