#include "LineElement.h"
#include "LineItem.h"
#include "LineLook.h"

namespace map_server
{
    void LineElement::load(void)
    {
        _loaded = true;

		std::auto_ptr<mongo::DBClientCursor> cursor = _iMap->getConnectionPtr()->query("diaphnea.line_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            int lookId = dbElement.getIntField("look_id");
            _look = dynamic_cast<const LineLook *>(_iMap->getLook(lookId));

            std::vector<mongo::BSONElement> dbLineItems = dbElement.getField("items").Array();
            int i, n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                std::string itemId = dbLineItems[i].OID().toString();
                LineItem *lineItem = _iMap->getLineItem(itemId);
				lineItem->setCurrentLook(_look->getLineLook());
				lineItem->setCurrentTextLook(_look->getTextLook());
                _itemVector.push_back(lineItem);
            }
        }
    }
}
