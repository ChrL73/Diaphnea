#include "PointElement.h"

namespace map_server
{
    void PointElement::load(void)
    {
        _loaded = true;

        auto cursor = _connectionPtr->query("diaphnea.point_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);
        }
    }
}
