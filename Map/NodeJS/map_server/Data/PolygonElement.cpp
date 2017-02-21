#include "PolygonElement.h"
#include "PolygonLook.h"
#include "FilledPolygonItem.h"

namespace map_server
{
    void PolygonElement::load(void)
    {
        _loaded = true;

		std::unique_ptr<mongo::DBClientCursor> cursor = _iMap->getConnectionPtr()->query("diaphnea.polygon_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            std::vector<mongo::BSONElement> dbLookIds = dbElement.getField("look_ids").Array();
            std::vector<const ItemLook *> fillLookVector;
            std::vector<const ItemLook *> textLookVector;

            int i, n = dbLookIds.size();
            for (i = 0; i < n; ++i)
            {
                int lookId = dbLookIds[i].Int();
                const PolygonLook *polygonLook = dynamic_cast<const PolygonLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(polygonLook);
                fillLookVector.push_back(polygonLook->getFillLook());
                textLookVector.push_back(polygonLook->getTextLook());
            }

            mongo::OID contourId = dbElement.getField("contour").OID();
            _filledPolygonItem = _iMap->getFilledPolygonItem(contourId);

            if (_filledPolygonItem == 0)
            {
                _error = true;
                return;
            }

            _filledPolygonItem->setCurrentLooks(fillLookVector);
			_filledPolygonItem->setCurrentTextLooks(textLookVector);
			_filledPolygonItem->setNameMap(&_nameMap);
            _filledPolygonItem->setElementIdForText(_id);
            _filledPolygonItem->setImportance(_importance);

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

                _lineItemVector.push_back(lineItem);
            }

            std::vector<mongo::BSONElement> dbCoveredElements = dbElement.getField("covered_elements").Array();
            n = dbCoveredElements.size();
            for (i = 0; i < n; ++i)
            {
                std::string coveredElement = dbCoveredElements[i].String();
                _coveredElementVector.push_back(coveredElement);
            }
        }
    }

    const PolygonLook *PolygonElement::getLook(int i) const
    {
        int n = _lookVector.size() - 1;
        if (i <= n) return _lookVector[i];
        return _lookVector[n];
    }
}
