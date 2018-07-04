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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class AttributeOrderElement
    {
        private readonly double _attributeValue;
        private readonly Element _element;

        internal AttributeOrderElement(double attributeValue, Element element)
        {
            _attributeValue = attributeValue;
            _element = element;
            MinAnswerIndex = -1;
        }

        internal int MinAnswerIndex { get; set; }
        internal double AttributeValue { get { return _attributeValue; } }
        internal Element Element { get { return _element; } }
    }
}
