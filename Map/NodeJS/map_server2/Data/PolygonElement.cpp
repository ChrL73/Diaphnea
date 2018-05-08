#include "PolygonElement.h"
#include "NameTranslation.h"

namespace map_server
{
    const PolygonLook *PolygonElement::getLook(int i) const
    {
        if (i >= _lookCount) i = _lookCount - 1;
        return reinterpret_cast<const PolygonLook *>(polygonLooks + *(intArrays + _looks + i));
    }

    int PolygonElement::getNameCount(int languageIndex) const
    {
        const NameTranslation *nameTranslation = reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex));
        return nameTranslation->getNameCount();
    }

    int PolygonElement::getLineCount(int languageIndex, int nameIndex) const
    {
        const NameTranslation *nameTranslation = reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex));
        return nameTranslation->getLineCount(nameIndex);
    }

    const char *PolygonElement::getNameLine(int languageIndex, int nameIndex, int lineIndex) const
    {
        const NameTranslation *nameTranslation = reinterpret_cast<const NameTranslation *>(nameTranslations + *(intArrays + _translations + languageIndex));
        return nameTranslation->getLine(nameIndex, lineIndex);
    }
}
