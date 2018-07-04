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

#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class Choice
    {
    private:
        // String
        int _choiceText;

        // String
        int _comment;

        // String
        int _mapId;

        // Array of doubles
        int _doubleCriterionValueCount;
        int _doubleCriterionValues;

        // Array of strings
        int _stringCriterionValueCount;
        int _stringCriterionValues;

        // Array of 3*n doubles (X1, Y1, Z1, X2, Y2, Z2, ... Xn, Yn, Zn)
        int _pointCriterionValueCount; // = n
        int _pointCriterionValues;

    public:
        const char *getChoiceText(void) const { return strings + _choiceText; }
        const char *getComment(void) const { return strings + _comment; }
        const char *getMapId(void) const { return strings + _mapId; }
        double getDoubleCriterionValue(void) const;
        const char *getStringCriterionValue(void) const;
        const double *getPointCriterionValue(void) const;
    };
}
