#include "QuestionProducer.h"
#include "RandomNumberGenerator.h"

// tmp?
#include "Choice.h"
#include "QuizData.h"

#include <iostream>

namespace produce_questions
{
    int QuestionProducer::run(void)
    {
        if (RandomNumberGenerator::verify() != 0)
        {
            std::cerr << "Error in RandomNumberGenerator::verify()" << std::endl;
            return -1;
        }

        Choice *choice = Choice::get(0);

        std::cout << choice->getChoiceText() << std::endl;
        std::cout << choice->getComment() << std::endl;
        std::cout << choice->getMapId() << std::endl;
        std::cout << choice->getDoubleCriterionValue() << std::endl;
        std::cout << choice->getStringCriterionValue() << std::endl;

        const double *pointCriterionValue = choice->getPointCriterionValue();

        std::cout << pointCriterionValue[0] << " "
                  << pointCriterionValue[1] << " "
                  << pointCriterionValue[2] << std::endl;

        choice = Choice::get(9);

        std::cout << choice->getChoiceText() << std::endl;
        std::cout << choice->getComment() << std::endl;
        std::cout << choice->getMapId() << std::endl;

        /*std::string json = "[";

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

        std::cout << json;*/

        return 0;
    }
}
