#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

// Avoid this warning: ‘template<class> class std::auto_ptr’ is deprecated
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#include "mongo/client/dbclient.h"
#pragma GCC diagnostic warning "-Wdeprecated-declarations"

#include <map>
#include <string>
#include <vector>

namespace map_server
{
    class Look;
    class LineItem;
    class FilledPolygonItem;
    class PointItem;
    class ItemLook;

    class IMap
    {
    public:
        virtual ~IMap() {}

        virtual mongo::DBClientConnection *getConnectionPtr(void) = 0;
        virtual std::vector<std::string> *getLanguageIdVectorPtr(void) = 0;
        virtual LineItem *getLineItem(const std::string& mongoId) = 0;
        virtual FilledPolygonItem *getFilledPolygonItem(const std::string& mongoId) = 0;
        virtual const Look *getLook(int lookId) = 0;
        virtual void addItemLook(const ItemLook *look) = 0;
        virtual void addPointItem(PointItem *pointItem) = 0;
    };
}
