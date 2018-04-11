using MongoDB.Bson;
using System;
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

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look.id },
                { "type", "line" },
                { "line_name", _lineName.getBsonDocument() },
                { "line_z_index", _look.lineZIndex },
                { "line_alpha", _look.lineAlpha },
                { "line_red", _look.lineRed },
                { "line_green", _look.lineGreen },
                { "line_blue", _look.lineBlue },
                { "line_size", _look.lineSize },
                { "text_name", _textName.getBsonDocument() },
                { "text_alpha", _look.textAlpha },
                { "text_red", _look.textRed },
                { "text_green", _look.textGreen },
                { "text_blue", _look.textBlue },
                { "text_size", _look.textSize }
            };

            return lookDocument;
        }
    }
}
