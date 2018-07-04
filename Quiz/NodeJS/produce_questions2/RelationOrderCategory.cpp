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

#include "RelationOrderCategory.h"
#include "RandomNumberGenerator.h"
#include "RelationOrderQuestion.h"

namespace produce_questions
{
    CompleteQuestion *RelationOrderCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        const RelationOrderQuestion *question = getQuestion(draw);

        CompleteQuestion *completeQuestion = question->getNewQuestion(choiceCount, distribParameter + _distribParameterCorrection, getMapParameters());

        return completeQuestion;
    }
}
