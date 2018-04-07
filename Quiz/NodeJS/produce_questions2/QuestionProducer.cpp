#include "QuestionProducer.h"
#include "RandomNumberGenerator.h"
#include "Level.h"
#include "Category.h"
#include "CompleteQuestion.h"

// tmp
/*#include "Choice.h"
#include "SimpleAnswerQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
#include "SimpleAnswerCategory.h"
#include "QuizData.h"*/

#include <iostream>
#include <vector>

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
        int x[] = { -1128541358, 2117959400 };
        double *y = reinterpret_cast<double *>(x);
        if (*y != 1.23456789012345678E+300)
        {
            std::cerr << "Unexpected double decoding" << std::endl;
            return -1;
        }

        /*Choice *choice = Choice::get(0);

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

        std::cout << "sizeof(Choice): " << sizeof(Choice) << std::endl;
        std::cout << "sizeof(SimpleAnswerQuestion): " << sizeof(SimpleAnswerQuestion) << std::endl;
        std::cout << "sizeof(MapParameters): " << sizeof(MapParameters) << std::endl;
        std::cout << "sizeof(MapSubParameters): " << sizeof(MapSubParameters) << std::endl;
        std::cout << "sizeof(SimpleAnswerCategory): " << sizeof(SimpleAnswerCategory) << std::endl;

        SimpleAnswerCategory *category = SimpleAnswerCategory::get(27);
        std::cout << category->getWeightIndex() << std::endl;
        std::cout << category->getMapParameters()->getFramingLevel() << std::endl;
        std::cout << category->getQuestionCount() << std::endl;
        std::cout << category->getQuestion(0)->getQuestion() << std::endl;
        std::cout << category->getChoiceCount() << std::endl;
        std::cout << category->getChoice(0)->getChoiceText() << std::endl;
        std::cout << category->getDistribParameterCorrection() << std::endl;
        std::cout << category->getProximityCriterionType() << std::endl;*/

        const Level *level = Level::instance();
        std::vector<Category *> categoryVector;

        int i, n = level->getSimpleAnswerCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getSimpleAnswerCategory(i));
            categoryVector.push_back(category);
        }

        /*n = level->getMultipleAnswerCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getMultipleAnswerCategory(i));
            categoryVector.push_back(category);
        }

        n = level->getRelationOrderCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getRelationOrderCategory(i));
            categoryVector.push_back(category);
        }

        n = level->getAttributeOrderCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getAttributeOrderCategory(i));
            categoryVector.push_back(category);
        }*/

        std::string json = "[";

        int questionCount = level->getQuestionCount();
        for (i = 0; i < questionCount; ++i)
        {
            Category *category = 0;
            int j, categoryCount = categoryVector.size();

            while (category == 0 || !category->isSuitable())
            {
                unsigned int draw = RandomNumberGenerator::getRandomInt(level->getWeightSum());

                for (j = 0; j < categoryCount; ++j)
                {
                    Category *jCategory = categoryVector[j];
                    if (draw < jCategory->getWeightIndex())
                    {
                        category = jCategory;
                        break;
                    }
                }
            }

            n = categoryCount / 10;
            if (n > 3) n = 3;
            category->reduceSuitability(n);
            for (j = 0; j < categoryCount; ++j)
            {
                if (category != categoryVector[j]) categoryVector[j]->increaseSuitability();
            }

            CompleteQuestion *question = category->getNewQuestion(level->getChoiceCount(), level->getDistribParameter());

            //json += question->getJson();
            //if (i != questionCount - 1) json += ",";

            delete question;
        }

        json += "]";

        std::cout << json;

        return 0;
    }
}
