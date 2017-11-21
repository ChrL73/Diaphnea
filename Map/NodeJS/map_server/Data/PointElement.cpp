#include "PointElement.h"
#include "PointLook.h"
#include "Point.h"
#include "PointItem.h"

namespace map_server
{
	void PointElement::load(void)
    {
        _loaded = true;

		std::unique_ptr<mongo::DBClientCursor> cursor = _iMap->getConnectionPtr()->query("diaphnea.point_elements", MONGO_QUERY("_id" << _mongoId), 1);
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

            std::vector<const ItemLook *> pointLookVector;
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

                const PointLook *pointLook = dynamic_cast<const PointLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(pointLook);
                pointLookVector.push_back(pointLook->getPointLook());
                textLookVector.push_back(pointLook->getTextLook());
            }

            int id = dbElement.getIntField("item_id");
            if (id < 0 || id > _maxIntDbValue)
            {
                _error = true;
                return;
            }

            mongo::BSONElement pointElt = dbElement.getField("point");
            if (pointElt.type() != mongo::Object)
            {
                _error = true;
                return;
            }
            mongo::BSONObj dbPoint = pointElt.Obj();

            mongo::BSONElement xElt = dbPoint.getField("x");
            mongo::BSONElement yElt = dbPoint.getField("y");
            if (xElt.type() != mongo::NumberDouble || yElt.type() != mongo::NumberDouble)
            {
                _error = true;
                return;
            }
            double x = xElt.Double();
            double y = yElt.Double();
            Point *point = new Point(x, -y);

            _item = new PointItem(id, point);
            _item->setCurrentLooks(pointLookVector);
			_item->setCurrentTextLooks(textLookVector);
            _item->setNameMap(&_nameMap);
            _item->setElementIdForText(_id);
            _item->setImportance(_importance);
            _item->setCurrentCategory(getCategory());
            _iMap->addPointItem(_item);
        }
        else
        {
            _error = true;
        }
    }

    const PointLook *PointElement::getLook(int i) const
    {
        int n = _lookVector.size() - 1;
        if (i <= n) return _lookVector[i];
        return _lookVector[n];
    }
}
