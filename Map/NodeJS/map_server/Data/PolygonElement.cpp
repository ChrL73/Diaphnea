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

            std::vector<const ItemLook *> fillLookVector;
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

                const PolygonLook *polygonLook = dynamic_cast<const PolygonLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(polygonLook);
                fillLookVector.push_back(polygonLook->getFillLook());
                textLookVector.push_back(polygonLook->getTextLook());
            }

            mongo::BSONElement contourElt = dbElement.getField("contour");
            if (contourElt.type() != mongo::jstOID)
            {
                _error = true;
                return;
            }
            mongo::OID contourId = contourElt.OID();

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
            _filledPolygonItem->setCurrentCategory(getCategory());

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

                _lineItemVector.push_back(lineItem);
            }

            mongo::BSONElement coveredElementsElt = dbElement.getField("covered_elements");
            if (coveredElementsElt.type() != mongo::Array)
            {
                _error = true;
                return;
            }
            std::vector<mongo::BSONElement> dbCoveredElements = coveredElementsElt.Array();

            n = dbCoveredElements.size();
            for (i = 0; i < n; ++i)
            {
                if (dbCoveredElements[i].type() != mongo::String)
                {
                    _error = true;
                    return;
                }
                std::string coveredElement = dbCoveredElements[i].String();
                _coveredElementVector.push_back(coveredElement);
            }
        }
        else
        {
            _error = true;
        }
    }

    const PolygonLook *PolygonElement::getLook(int i) const
    {
        int n = _lookVector.size() - 1;
        if (i <= n) return _lookVector[i];
        return _lookVector[n];
    }
}
