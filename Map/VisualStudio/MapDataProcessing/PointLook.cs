using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PointLook : Look
    {
        private readonly XmlPointLook _look;

        internal PointLook(XmlPointLook look)
        {
            _look = look;
        }

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look.id },
                { "type", "point" },
                { "point_z_index", _look.pointZIndex },
                { "point_alpha", _look.pointAlpha },
                { "point_red", _look.pointRed },
                { "point_green", _look.pointGreen },
                { "point_blue", _look.pointBlue },
                { "point_size", _look.pointSize },
                { "text_alpha", _look.textAlpha },
                { "text_red", _look.textRed },
                { "text_green", _look.textGreen },
                { "text_blue", _look.textBlue },
                { "text_size", _look.textSize }
            };

            return lookDocument; throw new NotImplementedException();
        }
    }
}
