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

#include "Choice.h"
#include "RandomNumberGenerator.h"
#include "QuizData.h"

namespace produce_questions
{
    double Choice::getDoubleCriterionValue(void) const
    {
        if (_doubleCriterionValueCount == 0) return 0.0;
        return *(doubleArrays + _doubleCriterionValues + RandomNumberGenerator::getRandomInt(_doubleCriterionValueCount));
    }

    const char *Choice::getStringCriterionValue(void) const
    {
        if (_stringCriterionValueCount == 0) return strings;
        return strings + *(intArrays + _stringCriterionValues + RandomNumberGenerator::getRandomInt(_stringCriterionValueCount));
    }

    const double *Choice::getPointCriterionValue(void) const
    {
        if (_pointCriterionValueCount == 0) return doubleArrays;
        return doubleArrays + _pointCriterionValues + 3 * RandomNumberGenerator::getRandomInt(_pointCriterionValueCount);
    }
}
