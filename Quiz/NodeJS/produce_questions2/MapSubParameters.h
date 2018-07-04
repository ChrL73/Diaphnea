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
    class MapSubParameters
    {
    private:
        // Int
        int _drawDepth;

        // String
        int _categorySelectionMode;

        // Array of ints
        int _categoryCount;
        int _categories;

        // int
        int _framingMode;

    public:
        int getDrawDepth(void) const { return _drawDepth; }
        const char *getCategorySelectionMode(void) const { return strings + _categorySelectionMode; }
        int getCategoryCount(void) const { return _categoryCount; }
        int getCategory(int i) const { return *(intArrays + _categories + i); }
        int getFramingMode(void) const { return _framingMode; }
    };
}
