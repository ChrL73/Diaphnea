#include "PointElement.h"
#include "PointLook.h"

namespace map_server
{
    void PointElement::load(void)
    {
        _loaded = true;

        auto cursor = _iMap->getConnectionPtr()->query("diaphnea.point_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            const char *lookId = dbElement.getStringField("look_id");
            _look = dynamic_cast<const PointLook *>(_iMap->getLook(lookId));
        }
    }
}
