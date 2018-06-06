using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace MapDataProcessing
{
    class PolygonLook : Look
    {
        private readonly XmlPolygonLook1 _look1;
        private readonly XmlPolygonLook2 _look2;
        private readonly ElementName _contourName;
        private readonly ElementName _fillName;
        private readonly ElementName _textName;

        internal PolygonLook(XmlPolygonLook1 look1, XmlNameSuffixes suffixes)
        {
            _look1 = look1;
            _look2 = null;
            _contourName = new ElementName(_look1.name, suffixes.stroke);
            _fillName = new ElementName(_look1.name, suffixes.fill);
            _textName = new ElementName(_look1.name, suffixes.text);
        }

        internal PolygonLook(XmlPolygonLook2 look2, XmlNameSuffixes suffixes)
        {
            _look1 = null;
            _look2 = look2;
            _contourName = new ElementName(look2.name, suffixes.stroke);
            _fillName = new ElementName(look2.name, suffixes.fill);
            _textName = new ElementName(look2.name, suffixes.text);
        }

        internal override string getJson()
        {
            int textAlpha = _look2 == null ? _look1.textAlpha : _look2.textAlpha;
            int textRed = _look2 == null ? _look1.textRed : _look2.textRed;
            int textGreen = _look2 == null ? _look1.textGreen : _look2.textGreen;
            int textBlue = _look2 == null ? _look1.textBlue : _look2.textBlue;
            string textSize = (_look2 == null ? _look1.textSize : _look2.textSize).ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
            int contourZIndex = _look2 == null ? _look1.contourZIndex : _look2.contourZIndex;
            int contourAlpha = _look2 == null ? _look1.contourAlpha : _look2.fillAlpha;
            int contourRed = _look2 == null ? _look1.contourRed : _look2.contourRed;
            int contourGreen = _look2 == null ? _look1.contourGreen : _look2.contourGreen;
            int contourBlue = _look2 == null ? _look1.contourBlue : _look2.contourBlue;
            string contourSize = (_look2 == null ? _look1.contourSize : _look2.contourSize).ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
            int fillZIndex = _look2 == null ? _look1.fillZIndex : _look2.fillZIndex;
            int fillAlpha = _look2 == null ? _look1.contourAlpha : _look2.fillAlpha;
            int fillRed = _look2 == null ? 255 - (int)((double)(255 - _look1.contourRed) * _look1.fillLightRatio) : _look2.fillRed;
            int fillGreen = _look2 == null ? 255 - (int)((double)(255 - _look1.contourGreen) * _look1.fillLightRatio) : _look2.fillGreen;
            int fillBlue = _look2 == null ? 255 - (int)((double)(255 - _look1.contourBlue) * _look1.fillLightRatio) : _look2.fillBlue;

            return String.Format(
                "{{\"id\":{0},\"name\":{1},\"zI\":0,\"a\":{2},\"r\":{3},\"g\":{4},\"b\":{5},\"size\":{6}}},{{\"id\":{7},\"name\":{8},\"zI\":{9},\"a\":{10},\"r\":{11},\"g\":{12},\"b\":{13},\"size\":{14}}},{{\"id\":{15},\"name\":{16},\"zI\":{17},\"a\":{18},\"r\":{19},\"g\":{20},\"b\":{21},\"size\":0}}",
                3 * Id, _textName.getJson(), textAlpha, textRed, textGreen, textBlue, textSize,
                3 * Id + 1, _contourName.getJson(), contourZIndex, contourAlpha, contourRed, contourGreen, contourBlue, contourSize,
                3 * Id + 2, _fillName.getJson(), fillZIndex, fillAlpha, fillRed, fillGreen, fillBlue);
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            CppOffset = codeGenerator.addPolygonLook(Id, _look1, _look2);

            return 0;
        }
    }
}
