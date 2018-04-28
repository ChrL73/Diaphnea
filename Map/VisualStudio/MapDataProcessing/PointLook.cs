using MongoDB.Bson;
using System;
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

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look.id },
                { "type", "point" },
                { "point_name", _pointName.getBsonDocument() },
                { "point_z_index", _look.pointZIndex },
                { "point_alpha", _look.pointAlpha },
                { "point_red", _look.pointRed },
                { "point_green", _look.pointGreen },
                { "point_blue", _look.pointBlue },
                { "point_size", _look.pointSize },
                { "text_name", _textName.getBsonDocument() },
                { "text_alpha", _look.textAlpha },
                { "text_red", _look.textRed },
                { "text_green", _look.textGreen },
                { "text_blue", _look.textBlue },
                { "text_size", _look.textSize }
            };

            return lookDocument;
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            CppOffset = codeGenerator.addPointLook(this);

            return 0;
        }
    }
}
