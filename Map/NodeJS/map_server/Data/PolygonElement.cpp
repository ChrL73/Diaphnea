#include "PolygonElement.h"
#include "PolygonLook.h"
#include "FilledPolygonItem.h"

namespace map_server
{
    void PolygonElement::load(void)
    {
        _loaded = true;

		std::auto_ptr<mongo::DBClientCursor> cursor = _iMap->getConnectionPtr()->query("diaphnea.polygon_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            int lookId = dbElement.getIntField("look_id");
            _look = dynamic_cast<const PolygonLook *>(_iMap->getLook(lookId));

            std::string contourId = dbElement.getField("contour").OID().toString();
            _filledPolygonItem = _iMap->getFilledPolygonItem(contourId);
            _filledPolygonItem->setCurrentLook(_look->getFillLook());
			_filledPolygonItem->setCurrentTextLook(_look->getTextLook());

            std::vector<mongo::BSONElement> dbLineItems = dbElement.getField("items").Array();
            int i, n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                std::string itemId = dbLineItems[i].OID().toString();
                LineItem *lineItem = _iMap->getLineItem(itemId);
                _lineItemVector.push_back(lineItem);
            }
        }
    }
}
