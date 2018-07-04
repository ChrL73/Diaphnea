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
    class MultipleAnswerQuestion
    {
    private:
        // String
        int _question;

        // String
        int _questionMapId;

        // 3 Arrays of strings (3 arrays of same size)
        int _answerCount;
        int _answers;
        int _comments;
        int _answerMapIds;

        // String
        int _excludedChoice;

        // ProximityCriterionTypeEnum
        int _proximityCriterionType;

        // 3 doubles
        double _pointCriterionValueX;
        double _pointCriterionValueY;
        double _pointCriterionValueZ;

    public:
        const char *getQuestion(void) const { return strings + _question; }
        const char *getQuestionMapId(void) const { return strings + _questionMapId; }
        int getAnswerCount(void) const { return _answerCount; }
        const char *getAnswer(int i) const { return strings + *(intArrays + _answers + i); }
        const char *getComment(int i) const { return strings + *(intArrays + _comments + i); }
        const char *getAnswerMapId(int i) const { return strings + *(intArrays + _answerMapIds + i); }
        const char *getExcludedChoice(void) const { return strings + _excludedChoice; }
        ProximityCriterionTypeEnum getProximityCriterionType(void) const { return static_cast<ProximityCriterionTypeEnum>(_proximityCriterionType); }
        double getPointCriterionValueX(void) const { return _pointCriterionValueX; }
        double getPointCriterionValueY(void) const { return _pointCriterionValueY; }
        double getPointCriterionValueZ(void) const { return _pointCriterionValueZ; }
    };
}
