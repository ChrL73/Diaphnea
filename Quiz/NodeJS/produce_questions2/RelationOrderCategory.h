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
    class MapParameters;
    class RelationOrderQuestion;
    class CompleteQuestion;

    class RelationOrderCategory
    {
    private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // Array of RelationOrderQuestion
        int _questionCount;
        int _questions;

        // Double
        double _distribParameterCorrection;

    public:
        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        int getQuestionCount(void) const { return _questionCount; }
        const RelationOrderQuestion *getQuestion(int i) const { return reinterpret_cast<const RelationOrderQuestion *>(relationOrderQuestions + *(intArrays + _questions + i)); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;
    };
}
