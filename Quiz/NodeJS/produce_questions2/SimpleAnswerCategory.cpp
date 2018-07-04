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

#include "SimpleAnswerCategory.h"
#include "RandomNumberGenerator.h"
#include "CompleteQuestion.h"
#include "SimpleAnswerQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
#include "Choice.h"

#include <map>
#include <cstring>
#include <cmath>

namespace produce_questions
{
    CompleteQuestion *SimpleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        const SimpleAnswerQuestion *question = getQuestion(draw);

        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), question->getQuestion(), produce_questions::SIMPLE, choiceCount);
        completeQuestion->addChoice(question->getAnswer(), question->getComment(), true);

        const MapSubParameters *questionParameters = getMapParameters()->getQuestionParameters();
        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getQuestionMapId(), questionParameters);
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getAnswerMapId(), answerParameters);

        std::vector<const Choice *> sortedWrongChoiceVector1;
        std::vector<const Choice *> sortedWrongChoiceVector2;

        if (_proximityCriterionType == produce_questions::STRING)
        {
            std::multimap<std::string, const Choice *> choiceMultimap;

            int i;
            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                choiceMultimap.insert(std::pair<std::string, const Choice *>(choice->getStringCriterionValue(), choice));
            }

            std::multimap<std::string, const Choice *>::const_iterator it1 = choiceMultimap.find(question->getStringCriterionValue());
            std::multimap<std::string, const Choice *>::const_iterator it2 = it1;
            ++it2;

            while (true)
            {
                if (it1 != choiceMultimap.begin())
                {
                    --it1;
                    const Choice *choice = (*it1).second;
                    if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                    {
                        sortedWrongChoiceVector1.push_back(choice);
                    }
                }
                if (it2 != choiceMultimap.end())
                {
                    const Choice *choice = (*it2).second;
                    if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                    {
                        sortedWrongChoiceVector2.push_back(choice);
                    }
                    ++it2;
                }
                if (it1 == choiceMultimap.begin() && it2 == choiceMultimap.end()) break;
            }
        }
        else if (_proximityCriterionType == produce_questions::NUMBER)
        {
            std::multimap<double, const Choice *> choiceMultimap;

            int i;
            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                choiceMultimap.insert(std::pair<double, const Choice *>(choice->getDoubleCriterionValue(), choice));
            }

            std::multimap<double, const Choice *>::const_iterator it1 = choiceMultimap.find(question->getDoubleCriterionValue());
            std::multimap<double, const Choice *>::const_iterator it2 = it1;
            ++it2;

            while (true)
            {
                if (it1 != choiceMultimap.begin())
                {
                    --it1;
                    const Choice *choice = (*it1).second;
                    if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                    {
                        sortedWrongChoiceVector1.push_back(choice);
                    }
                }
                if (it2 != choiceMultimap.end())
                {
                    const Choice *choice = (*it2).second;
                    if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                    {
                        sortedWrongChoiceVector2.push_back(choice);
                    }
                    ++it2;
                }
                if (it1 == choiceMultimap.begin() && it2 == choiceMultimap.end()) break;
            }
        }
        else if (_proximityCriterionType == produce_questions::POINT_3D)
        {
            double x0 = question->getPointCriterionValueX();
            double y0 = question->getPointCriterionValueY();
            double z0 = question->getPointCriterionValueZ();

            std::multimap<double, const Choice *> choiceMultimap;

            int i;
            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 )
                {
                    double x = choice->getPointCriterionValue()[0];
                    double y = choice->getPointCriterionValue()[1];
                    double z = choice->getPointCriterionValue()[2];

                    double dx = x - x0;
                    double dy = y - y0;
                    double dz = z - z0;

                    double d = sqrt(dx*dx +dy*dy + dz*dz);
                    choiceMultimap.insert(std::pair<double, const Choice *>(d, choice));
                }
            }

            std::multimap<double, const Choice *>::iterator it = choiceMultimap.begin();
            for (; it != choiceMultimap.end(); ++it)
            {
                const Choice *choice = (*it).second;
                if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                {
                    sortedWrongChoiceVector1.push_back(choice);
                }
            }
        }
        else
        {
            int i;
            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                if (strcmp(choice->getChoiceText(), question->getAnswer()) != 0 && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                {
                    sortedWrongChoiceVector1.push_back(choice);
                }
            }
        }

        std::set<unsigned int> excludedValues;
        int wrongChoiceCount1 = sortedWrongChoiceVector1.size();
        int wrongChoiceCount2 = sortedWrongChoiceVector2.size();

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
            const Choice *wrongChoice = sortedWrongChoiceVector1[draw];
            completeQuestion->addChoice(wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        excludedValues.clear();
        for (i = 0; i < n2; ++i)
        {
            if (_proximityCriterionType == produce_questions::NONE) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount2, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            const Choice *wrongChoice = sortedWrongChoiceVector2[draw];
            completeQuestion->addChoice (wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
