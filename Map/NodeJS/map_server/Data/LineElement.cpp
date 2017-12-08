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

            if (!loadCommon(dbElement))
            {
                _error = true;
                return;
            }

            mongo::BSONElement lookIdsElt = dbElement.getField("look_ids");
            if (lookIdsElt.type() != mongo::Array)
            {
                _error = true;
                return;
            }
            std::vector<mongo::BSONElement> dbLookIds = lookIdsElt.Array();

            std::vector<const ItemLook *> lineLookVector;
            std::vector<const ItemLook *> textLookVector;

            int i, n = dbLookIds.size();
            for (i = 0; i < n; ++i)
            {
                if (dbLookIds[i].type() != mongo::NumberInt)
                {
                    _error = true;
                    return;
                }
                int lookId = dbLookIds[i].Int();

                const LineLook *lineLook = dynamic_cast<const LineLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(lineLook);
                lineLookVector.push_back(lineLook->getLineLook());
                textLookVector.push_back(lineLook->getTextLook());
            }

            mongo::BSONElement itemsElt = dbElement.getField("items");
            if (itemsElt.type() != mongo::Array)
            {
                _error = true;
                return;
            }
            std::vector<mongo::BSONElement> dbLineItems = itemsElt.Array();

            n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                if (dbLineItems[i].type() != mongo::jstOID)
                {
                    _error = true;
                    return;
                }
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
                lineItem->setElementIdForText(_id);
                lineItem->setImportance(_importance);
                lineItem->setElementForFraming(this);
				_itemVector.push_back(lineItem);
            }
        }
        else
        {
            _error = true;
        }
    }
}
