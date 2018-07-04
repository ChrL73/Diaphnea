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
    class CompleteQuestion;

    class AttributeOrderCategory
    {
    private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // String
        int _questionText;

        //  4 arrays of same size
        int _choiceCount;
        int _choiceTexts; // Array of strings
        int _choiceComments; // Array of strings
        int _choiceMinIndexes; // Array of ints
        int _choiceMapIds; // Array of strings

        // Double
        double _distribParameterCorrection;

        // Int
        int _maxIndex;

    public:
        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        const char *getQuestionText(void) const { return strings + _questionText; }

        int getChoiceCount(void) const { return _choiceCount; }
        const char *getChoiceText(int i) const { return strings + *(intArrays + _choiceTexts + i); }
        const char *getChoiceComment(int i) const { return strings + *(intArrays + _choiceComments + i); }
        int getChoiceMinIndex(int i) const { return *(intArrays + _choiceMinIndexes + i); }
        const char *getChoiceMapId(int i) const { return strings + *(intArrays + _choiceMapIds + i); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        int getMaxIndex(void) const { return _maxIndex; }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;
    };
}
