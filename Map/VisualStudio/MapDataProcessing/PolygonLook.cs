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
        private readonly XmlPolygonLook1 _look1;
        private readonly XmlPolygonLook2 _look2;

        internal PolygonLook(XmlPolygonLook1 look1)
        {
            _look1 = look1;
            _look2 = null;
        }

        internal PolygonLook(XmlPolygonLook2 look2)
        {
            _look1 = null;
            _look2 = look2;
        }

        internal override BsonDocument getBsonDocument()
        {
            BsonDocument lookDocument = new BsonDocument()
            {
                { "id", Id },
                { "xml_id", _look2 == null ? _look1.id : _look2.id },
                { "type", "polygon" },
                { "contour_z_index", _look2 == null ? _look1.contourZIndex : _look2.contourZIndex },
                { "contour_alpha", _look2 == null ? _look1.contourAlpha : _look2.contourAlpha },
                { "contour_red", _look2 == null ? _look1.contourRed : _look2.contourRed },
                { "contour_green", _look2 == null ? _look1.contourGreen : _look2.contourGreen },
                { "contour_blue", _look2 == null ? _look1.contourBlue : _look2.contourBlue },
                { "contour_size", _look2 == null ? _look1.contourSize : _look2.contourSize },
                { "fill_alpha", _look2 == null ? _look1.contourAlpha : _look2.fillAlpha },
                { "fill_red", _look2 == null ? 255 - (int)((double)(255 - _look1.contourRed) * _look1.fillLightRatio) : _look2.fillRed },
                { "fill_green", _look2 == null ? 255 - (int)((double)(255 - _look1.contourGreen) * _look1.fillLightRatio) : _look2.fillGreen },
                { "fill_blue", _look2 == null ? 255 - (int)((double)(255 - _look1.contourBlue) * _look1.fillLightRatio) : _look2.fillBlue },
                { "fill_z_index", _look2 == null ? _look1.fillZIndex : _look2.fillZIndex },
                { "text_alpha", _look2 == null ? _look1.textAlpha : _look2.textAlpha },
                { "text_red", _look2 == null ? _look1.textRed : _look2.textRed },
                { "text_green", _look2 == null ? _look1.textGreen : _look2.textGreen },
                { "text_blue", _look2 == null ? _look1.textBlue : _look2.textBlue },
                { "text_size", _look2 == null ? _look1.textSize : _look2.textSize }
            };

            return lookDocument;
        }
    }
}
