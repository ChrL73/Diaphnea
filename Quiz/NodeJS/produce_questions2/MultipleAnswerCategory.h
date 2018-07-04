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
#include "ProximityCriterionTypeEnum.h"

namespace produce_questions
{
    class MapParameters;
    class MultipleAnswerQuestion;
    class Choice;
    class CompleteQuestion;

    class MultipleAnswerCategory
    {
    private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // Array of MultipleAnswerQuestion
        int _questionCount;
        int _questions;

        // Array of Choice
        int _choiceCount;
        int _choices;

        // Double
        double _distribParameterCorrection;

        // ProximityCriterionTypeEnum
        int _proximityCriterionType;

    public:
        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        int getQuestionCount(void) const { return _questionCount; }
        const MultipleAnswerQuestion *getQuestion(int i) const { return reinterpret_cast<const MultipleAnswerQuestion *>(multipleAnswerQuestions + *(intArrays + _questions + i)); }

        int getChoiceCount(void) const { return _choiceCount; }
        const Choice *getChoice(int i) const { return reinterpret_cast<const Choice *>(choices + *(intArrays + _choices + i)); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        ProximityCriterionTypeEnum getProximityCriterionType(void) const { return static_cast<ProximityCriterionTypeEnum>(_proximityCriterionType); }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;
    };
}

