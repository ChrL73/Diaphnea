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

#include "AttributeOrderCategory.h"
#include "CompleteQuestion.h"
#include "RandomNumberGenerator.h"
#include "MapParameters.h"
#include "MapSubParameters.h"

namespace produce_questions
{
    CompleteQuestion *AttributeOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), getQuestionText(), produce_questions::SIMPLE, choiceCount);

        int draw = RandomNumberGenerator::getRandomInt(_maxIndex + 1);
        completeQuestion->addChoice(getChoiceText(draw), getChoiceComment(draw), true);

        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(draw), answerParameters);

        int minIndex = getChoiceMinIndex(draw);
        int n = _choiceCount;
        std::set<unsigned int> excludedValues;

        int i;
        for (i = 0; i < choiceCount - 1; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(n - minIndex, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice(getChoiceText(minIndex + draw), getChoiceComment(minIndex + draw), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(getChoiceMapId(minIndex + draw), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
