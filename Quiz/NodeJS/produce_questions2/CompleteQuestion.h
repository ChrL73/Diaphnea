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

#include "QuestionMultiplicityEnum.h"

#include <string>
#include <vector>
#include <set>

namespace produce_questions
{
    class TextAndComment;
    class MapIdInfo;
    class MapParameters;
    class MapSubParameters;

    class CompleteQuestion
    {
    private:
        const MapParameters * const _mapParameters;
        const std::string _question;
        const QuestionMultiplicityEnum _multiplicity;
        std::vector<TextAndComment *> _choiceVector;
        std::vector<MapIdInfo *> _mapIdVector;
        std::set<int> _rightAnswerSet;
        std::set<unsigned int> _filledLocationSet;
        std::string _json;

    public:
        CompleteQuestion(const MapParameters *mapParameters, const std::string& question, QuestionMultiplicityEnum multiplicity, int choiceCount);
        ~CompleteQuestion();

        void addChoice(const std::string& text, const std::string& comment, bool rightAnswer);
        void addMapId(const std::string& id, const MapSubParameters *subParameters);
        const std::string& getJson(void);

    };
}
