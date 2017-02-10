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

            loadCommon(dbElement);

            std::vector<mongo::BSONElement> dbLookIds = dbElement.getField("look_ids").Array();
            std::vector<const ItemLook *> pointLookVector;
            std::vector<const ItemLook *> textLookVector;

            int i, n = dbLookIds.size();
            for (i = 0; i < n; ++i)
            {
                int lookId = dbLookIds[i].Int();
                const PointLook *pointLook = dynamic_cast<const PointLook *>(_iMap->getLook(lookId));
                _lookVector.push_back(pointLook);
                pointLookVector.push_back(pointLook->getPointLook());
                textLookVector.push_back(pointLook->getTextLook());
            }

            int id = dbElement.getIntField("item_id");

            mongo::BSONObj dbPoint = dbElement.getField("point").Obj();
            double x = dbPoint.getField("x").Double();
            double y = dbPoint.getField("y").Double();
            Point *point = new Point(x, -y);

            _item = new PointItem(id, point);
            _item->setCurrentLooks(pointLookVector);
			_item->setCurrentTextLooks(textLookVector);
            _item->setNameMap(&_nameMap);
            _item->setElementIdForText(_id);
            _item->setImportance(_importance);
            _iMap->addPointItem(_item);
        }
    }

    const PointLook *PointElement::getLook(int i) const
    {
        int n = _lookVector.size() - 1;
        if (i <= n) return _lookVector[i];
        return _lookVector[n];
    }
}
