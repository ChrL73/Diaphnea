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
            std::vector<const ItemLook *> lineLookVector;
            std::vector<const ItemLook *> textLookVector;

            int i, n = dbLookIds.size();
            for (i = 0; i < n; ++i)
            {
                int lookId = dbLookIds[i].Int();
                const LineLook *lineLook = dynamic_cast<const LineLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(lineLook);
                lineLookVector.push_back(lineLook->getLineLook());
                textLookVector.push_back(lineLook->getTextLook());
            }

            std::vector<mongo::BSONElement> dbLineItems = dbElement.getField("items").Array();
            n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                mongo::OID itemId = dbLineItems[i].OID();
                LineItem *lineItem = _iMap->getLineItem(itemId);

				if (lineItem == 0)
				{
					_error = true;
					return;
				}

				lineItem->setCurrentLooks(lineLookVector);
				lineItem->setCurrentTextLooks(textLookVector);
				lineItem->setNameMap(&_nameMap);
                //lineItem->setElementIdForText(_numericalId);
                lineItem->setElementIdForText(_id);
                lineItem->setImportance(_importance);
				_itemVector.push_back(lineItem);
            }
        }
    }
}
