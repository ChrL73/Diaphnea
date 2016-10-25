using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class LineLook : Look
    {
        private readonly XmlLineLook _look;

        internal LineLook(XmlLineLook look)
        {
            _look = look;
        }

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", _look.id },
                { "type", "line" },
                { "line_z_index", Int32.Parse(_look.lineZIndex) },
                { "line_alpha", _look.lineAlpha },
                { "line_red", _look.lineRed },
                { "line_green", _look.lineGreen },
                { "line_blue", _look.lineBlue },
                { "line_size", _look.lineSize },
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
