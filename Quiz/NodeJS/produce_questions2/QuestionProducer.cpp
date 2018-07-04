/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#include "QuestionProducer.h"
#include "RandomNumberGenerator.h"
#include "Level.h"
#include "Category.h"
#include "CompleteQuestion.h"

#include <iostream>
#include <vector>
#include <map>

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

        const Level *level = Level::instance();
        std::multimap<unsigned int, Category *> categoryMap;

        int i, n = level->getSimpleAnswerCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getSimpleAnswerCategory(i));
            categoryMap.insert(std::pair<unsigned int, Category *>(category->getWeightIndex(), category));
        }

        n = level->getMultipleAnswerCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getMultipleAnswerCategory(i));
            categoryMap.insert(std::pair<unsigned int, Category *>(category->getWeightIndex(), category));
        }

        n = level->getRelationOrderCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getRelationOrderCategory(i));
            categoryMap.insert(std::pair<unsigned int, Category *>(category->getWeightIndex(), category));
        }

        n = level->getAttributeOrderCategoryCount();
        for (i = 0; i < n; ++i)
        {
            Category *category = new Category(level->getAttributeOrderCategory(i));
            categoryMap.insert(std::pair<unsigned int, Category *>(category->getWeightIndex(), category));
        }

        std::vector<Category *> categoryVector;
        std::multimap<unsigned int, Category *>::iterator it = categoryMap.begin();
        for (; it != categoryMap.end(); ++it) categoryVector.push_back((*it).second);

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

            json += question->getJson();
            if (i != questionCount - 1) json += ",";

            delete question;
        }

        json += "]";

        std::cout << json;

        n = categoryVector.size();
        for (i = 0; i < n; ++i) delete categoryVector[i];

        return 0;
    }
}
