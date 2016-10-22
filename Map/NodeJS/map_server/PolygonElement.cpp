#include "PolygonElement.h"

namespace map_server
{
    void PolygonElement::load(void)
    {
        _loaded = true;

        auto cursor = _connectionPtr->query("diaphnea.polygon_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);
        }
    }
}
