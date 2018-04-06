#include "QuestionProducer.h"
#include "RandomNumberGenerator.h"

// tmp?
#include "Choice.h"
#include "SimpleAnswerQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
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

        // In the generated code, some doubles (8 bytes) are encoded as 2 ints (2 * 4 bytes).
        // The following lines verify that such a pair of ints is decoded as expected
        int i[] = { -1128541358, 2117959400 };
        double *p = reinterpret_cast<double *>(i);
        if (*p != 1.23456789012345678E+300)
        {
            std::cerr << "Unexpected double decoding" << std::endl;
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

        SimpleAnswerQuestion *question = SimpleAnswerQuestion::get(0);
        std::cout << question->getQuestion() << std::endl;
        std::cout << question->getAnswer() << std::endl;
        std::cout << question->getProximityCriterionType() << std::endl;
        std::cout << question->getStringCriterionValue() << std::endl;
        std::cout << question->getDoubleCriterionValue() << std::endl;

        question = SimpleAnswerQuestion::get(3488);
        std::cout << question->getQuestion() << std::endl;
        std::cout << question->getAnswer() << std::endl;
        std::cout << question->getPointCriterionValueX() << std::endl;
        std::cout << question->getPointCriterionValueY() << std::endl;
        std::cout << question->getPointCriterionValueZ() << std::endl;

        MapParameters *mapParameters = MapParameters::get(5);
        std::cout << mapParameters->getFramingLevel() << std::endl;
        std::cout << mapParameters->getAnswerParameters()->getDrawDepth() << std::endl;
        std::cout << mapParameters->getAnswerParameters()->getCategorySelectionMode() << std::endl;

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
