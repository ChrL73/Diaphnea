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

            int lookId = dbElement.getIntField("look_id");
            _look = dynamic_cast<const LineLook *>(_iMap->getLook(lookId));

            std::vector<mongo::BSONElement> dbLineItems = dbElement.getField("items").Array();
            int i, n = dbLineItems.size();
            for (i = 0; i < n; ++i)
            {
                std::string itemId = dbLineItems[i].OID().toString();
                LineItem *lineItem = _iMap->getLineItem(itemId);
                _itemVector.push_back(lineItem);
            }
        }
    }
}
