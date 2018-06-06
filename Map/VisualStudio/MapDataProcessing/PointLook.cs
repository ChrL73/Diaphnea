using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class PointLook : Look
    {
        private readonly XmlPointLook _look;
        private readonly ElementName _pointName;
        private readonly ElementName _textName;

        internal PointLook(XmlPointLook look, XmlNameSuffixes suffixes)
        {
            _look = look;
            _pointName = new ElementName(look.name, suffixes.fill);
            _textName = new ElementName(look.name, suffixes.text);
        }

        internal override string getJson()
        {
            string textSize = _look.textSize.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
            string pointSize = _look.pointSize.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));

            return String.Format(
                "{{\"id\":{0},\"name\":{1},\"zI\":0,\"a\":{2},\"r\":{3},\"g\":{4},\"b\":{5},\"size\":{6}}},{{\"id\":{7},\"name\":{8},\"zI\":{9},\"a\":{10},\"r\":{11},\"g\":{12},\"b\":{13},\"size\":{14}}}",
                3 * Id, _textName.getJson(), _look.textAlpha, _look.textRed, _look.textGreen, _look.textBlue, textSize,
                3 * Id + 1, _pointName.getJson(), _look.pointZIndex, _look.pointAlpha, _look.pointRed, _look.pointGreen, _look.pointBlue, pointSize);
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            CppOffset = codeGenerator.addPointLook(Id, _look);

            return 0;
        }
    }
}
