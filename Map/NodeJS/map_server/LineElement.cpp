#include "LineElement.h"
#include "LineLook.h"

namespace map_server
{
    void LineElement::load(void)
    {
        _loaded = true;

        auto cursor = _iMap->getConnectionPtr()->query("diaphnea.line_elements", MONGO_QUERY("_id" << _mongoId), 1);
        if (cursor->more())
        {
            mongo::BSONObj dbElement = cursor->next();

            loadCommon(dbElement);

            const char *lookId = dbElement.getStringField("look_id");
            _look = dynamic_cast<const LineLook *>(_iMap->getLook(lookId));
        }
    }
}
