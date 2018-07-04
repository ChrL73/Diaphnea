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

#include <string>

namespace produce_questions
{
    class TextAndComment
    {
    private:
        const std::string _text;
        const std::string _comment;

    public:
        TextAndComment(const std::string& text, const std::string& comment) :
            _text(text), _comment(comment) {}

        const std::string& getText(void) const { return _text; }
        const std::string& getComment(void) const { return _comment; }

    };
}
