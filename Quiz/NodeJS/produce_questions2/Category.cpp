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

#include "Category.h"
#include "SimpleAnswerCategory.h"
#include "MultipleAnswerCategory.h"
#include "RelationOrderCategory.h"
#include "AttributeOrderCategory.h"

namespace produce_questions
{
    bool Category::isSuitable(void)
    {
        if (_suitabilityCounter == 0) return true;
        --_suitabilityCounter;
        return false;
    }

    unsigned int Category::getWeightIndex(void) const
    {
        if (_simpleAnswerCategory != 0) return _simpleAnswerCategory->getWeightIndex();
        if (_multipleAnswerCategory != 0) return _multipleAnswerCategory->getWeightIndex();
        if (_relationOrderCategory != 0) return _relationOrderCategory->getWeightIndex();
        if (_attributeOrderCategory != 0) return _attributeOrderCategory->getWeightIndex();

        return 0;
    }

    CompleteQuestion *Category::getNewQuestion(int choiceCount, double distribParameter)
    {
        if (_simpleAnswerCategory != 0) return _simpleAnswerCategory->getNewQuestion(choiceCount, distribParameter);
        if (_multipleAnswerCategory != 0) return _multipleAnswerCategory->getNewQuestion(choiceCount, distribParameter);
        if (_relationOrderCategory != 0) return _relationOrderCategory->getNewQuestion(choiceCount, distribParameter);
        if (_attributeOrderCategory != 0) return _attributeOrderCategory->getNewQuestion(choiceCount, distribParameter);

        return 0;
    }
}
