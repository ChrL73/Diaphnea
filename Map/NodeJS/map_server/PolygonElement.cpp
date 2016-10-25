#include "PolygonElement.h"
#include "PolygonLook.h"

namespace map_server
{
    void PolygonElement::load(void)
    {
        _loaded = true;

        auto cursor = _iMap->getConnectionPtr()->query("diaphnea.polygon_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            const char *lookId = dbElement.getStringField("look_id");
            _look = dynamic_cast<const PolygonLook *>(_iMap->getLook(lookId));
        }
    }
}
