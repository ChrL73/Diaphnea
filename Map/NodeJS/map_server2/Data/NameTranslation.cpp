#include "NameTranslation.h"
#include "ElementName.h"
#include "MapData.h"

namespace map_server
{
    int NameTranslation::getLineCount(int nameIndex) const
    {
        const ElementName *elementName = reinterpret_cast<const ElementName *>(elementNames + *(intArrays + _names + nameIndex));
        return elementName->getLineCount();
    }

    const char *NameTranslation::getLine(int nameIndex, int lineIndex) const
    {
        const ElementName *elementName = reinterpret_cast<const ElementName *>(elementNames + *(intArrays + _names + nameIndex));
        return elementName->getLine(lineIndex);
    }
}
