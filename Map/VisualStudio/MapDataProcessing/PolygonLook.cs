using MongoDB.Bson;
using System;
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

        internal XmlPolygonLook1 Look1 { get { return _look1; } }
        internal XmlPolygonLook2 Look2 { get { return _look2; } }

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

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look2 == null ? _look1.id : _look2.id },
                { "type", "polygon" },
                { "contour_name", _contourName.getBsonDocument() },
                { "contour_z_index", _look2 == null ? _look1.contourZIndex : _look2.contourZIndex },
                { "contour_alpha", _look2 == null ? _look1.contourAlpha : _look2.contourAlpha },
                { "contour_red", _look2 == null ? _look1.contourRed : _look2.contourRed },
                { "contour_green", _look2 == null ? _look1.contourGreen : _look2.contourGreen },
                { "contour_blue", _look2 == null ? _look1.contourBlue : _look2.contourBlue },
                { "contour_size", _look2 == null ? _look1.contourSize : _look2.contourSize },
                { "fill_name", _fillName.getBsonDocument() },
                { "fill_alpha", _look2 == null ? _look1.contourAlpha : _look2.fillAlpha },
                { "fill_red", _look2 == null ? 255 - (int)((double)(255 - _look1.contourRed) * _look1.fillLightRatio) : _look2.fillRed },
                { "fill_green", _look2 == null ? 255 - (int)((double)(255 - _look1.contourGreen) * _look1.fillLightRatio) : _look2.fillGreen },
                { "fill_blue", _look2 == null ? 255 - (int)((double)(255 - _look1.contourBlue) * _look1.fillLightRatio) : _look2.fillBlue },
                { "fill_z_index", _look2 == null ? _look1.fillZIndex : _look2.fillZIndex },
                { "text_name", _textName.getBsonDocument() },
                { "text_alpha", _look2 == null ? _look1.textAlpha : _look2.textAlpha },
                { "text_red", _look2 == null ? _look1.textRed : _look2.textRed },
                { "text_green", _look2 == null ? _look1.textGreen : _look2.textGreen },
                { "text_blue", _look2 == null ? _look1.textBlue : _look2.textBlue },
                { "text_size", _look2 == null ? _look1.textSize : _look2.textSize }
            };

            return lookDocument;
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            CppOffset = codeGenerator.addPolygonLook(this);

            return 0;
        }
    }
}
