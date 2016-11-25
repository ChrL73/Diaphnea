using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class PolygonLook : Look
    {
        private readonly XmlPolygonLook _look;

        internal PolygonLook(XmlPolygonLook look)
        {
            _look = look;
        }

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look.id },
                { "type", "polygon" },
                { "contour_z_index", _look.contourZIndex },
                { "contour_alpha", _look.contourAlpha },
                { "contour_red", _look.contourRed },
                { "contour_green", _look.contourGreen },
                { "contour_blue", _look.contourBlue },
                { "contour_size", _look.contourSize },
                { "fill_light_ratio", _look.fillLightRatio },
                { "fill_z_index", _look.fillZIndex },
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
