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
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class LineLook : Look
    {
        private readonly XmlLineLook _look;
        private readonly ElementName _lineName;
        private readonly ElementName _textName;

        internal LineLook(XmlLineLook look, XmlNameSuffixes suffixes)
        {
            _look = look;
            _lineName = new ElementName(look.name, suffixes.stroke);
            _textName = new ElementName(look.name, suffixes.text);
        }

        internal override string getJson()
        {
            string textSize = _look.textSize.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
            string lineSize = _look.lineSize.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));

            return String.Format(
                "{{\"id\":{0},\"name\":{1},\"zI\":0,\"a\":{2},\"r\":{3},\"g\":{4},\"b\":{5},\"size\":{6}}},{{\"id\":{7},\"name\":{8},\"zI\":{9},\"a\":{10},\"r\":{11},\"g\":{12},\"b\":{13},\"size\":{14}}}",
                3 * Id, _textName.getJson(), _look.textAlpha, _look.textRed, _look.textGreen, _look.textBlue, textSize,
                3 * Id + 1, _lineName.getJson(), _look.lineZIndex, _look.lineAlpha, _look.lineRed, _look.lineGreen, _look.lineBlue, lineSize);
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            CppOffset = codeGenerator.addLineLook(Id, _look);

            return 0;
        }
    }
}
