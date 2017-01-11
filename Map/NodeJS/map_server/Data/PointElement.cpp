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

            int lookId = dbElement.getIntField("look_id");
            _look = dynamic_cast<const PointLook *>(_iMap->getLook(lookId));

            int id = dbElement.getIntField("item_id");

            mongo::BSONObj dbPoint = dbElement.getField("point").Obj();
            double x = dbPoint.getField("x").Double();
            double y = dbPoint.getField("y").Double();
            Point *point = new Point(x, -y);

            _item = new PointItem(id, point);
            _item->setCurrentLook(_look->getPointLook());
			_item->setCurrentTextLook(_look->getTextLook());
            _item->setNameMap(&_nameMap);
            _item->setElementIdForText(_id);
            _iMap->addPointItem(_item);
        }
    }
}
