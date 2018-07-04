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
    class CompleteQuestion;
    class MapParameters;

    class RelationOrderQuestion
    {
    private:
        // String
        int _question;

        // String
        int _mapId;

        // 2 Arrays of strings (2 arrays of same size)
        int _choiceCount;
        int _choiceTexts;
        int _choiceMapIds;

    public:
        const char *getQuestion(void) const { return strings + _question; }
        const char *getMapId(void) const { return strings + _mapId; }

        int getChoiceCount(void) const { return _choiceCount; }
        const char *getChoiceText(int i) const { return strings + *(intArrays + _choiceTexts + i); }
        const char *getChoiceMapId(int i) const { return strings + *(intArrays + _choiceMapIds + i); }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter, const MapParameters *mapParameters) const;
    };
}
