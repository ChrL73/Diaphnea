#include "LineElement.h"
#include "LineItem.h"
#include "LineLook.h"

namespace map_server
{
    void LineElement::load(void)
    {
        _loaded = true;

		std::unique_ptr<mongo::DBClientCursor> cursor = _iMap->getConnectionPtr()->query("diaphnea.line_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            std::vector<mongo::BSONElement> dbLookIds = dbElement.getField("look_ids").Array();
            int i, n = dbLookIds.size();
            for (i = 0; i < n; ++i)
            {
                int lookId = dbLookIds[i].Int();
                _lookVector.push_back(dynamic_cast<const LineLook *>(_iMap->getLook(lookId)));
            }

            std::vector<mongo::BSONElement> dbLineItems = dbElement.getField("items").Array();
            n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                std::string itemId = dbLineItems[i].OID().toString();
                LineItem *lineItem = _iMap->getLineItem(itemId);
				lineItem->setCurrentLook(_lookVector[0]->getLineLook());
				lineItem->setCurrentTextLook(_lookVector[0]->getTextLook());
				lineItem->setNameMap(&_nameMap);
                //lineItem->setElementIdForText(_numericalId);
                lineItem->setElementIdForText(_id);
                lineItem->setImportance(_importance);
				_itemVector.push_back(lineItem);
            }
        }
    }
}
