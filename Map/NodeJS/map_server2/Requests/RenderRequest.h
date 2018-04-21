#pragma once

#include <vector>
#include <map>
#include <set>
#include <string>

#include "Request.h"

typedef struct FT_FaceRec_ *FT_Face;

namespace map_server
{
    class SvgCreator;
    class SvgCustomColor;

    class RenderRequest : public Request
    {
    private:
        const char * const _mapId;
        const char * const _languageId;
        const double _widthInPixels;
        const double _heightInPixels;
        const int _lookIndex;
        const std::vector<const char *> _elementIds;
        const std::map<int, SvgCustomColor *> _customColorMap;
        const bool _focusSetByClient;
        double _scale;
        double _xFocus;
        double _yFocus;
        const int _framingLevel;
        const std::set<std::string> _framingExceptions;
        bool _createPotentialImage;
        SvgCreator *_svgCreator;
        //bool _testMode;

        void execute(void);
        //bool displayText(void);
        //void setTextInfo(ItemCopy *itemCopy, ItemCopyBuilder *itemCopyBuilder, double sizeFactor, FT_Face face);

    public:
        RenderRequest(const char *socketId, const char *requestId, const char *mapId, const char *languageId, double widthInPixels, double heightInPixels,
                      int lookIndex, const std::vector<const char *>& elementIds, const std::map<int, SvgCustomColor *>& customColorMap, int framingLevel,
                      const std::set<std::string>& framingExceptions) :
            Request(socketId, requestId), _mapId(mapId), _languageId(languageId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _lookIndex(lookIndex),
            _elementIds(elementIds), _customColorMap(customColorMap), _focusSetByClient(false), _scale(1.0), _xFocus(0.0), _yFocus(0.0), _framingLevel(framingLevel),
            _framingExceptions(framingExceptions), _createPotentialImage(false), _svgCreator(0)/*, _testMode(false)*/ {}

        RenderRequest(const char *socketId, const char *requestId, const char *mapId, const char *languageId, double widthInPixels, double heightInPixels,
                      int lookIndex, const std::vector<const char *>& elementIds, const std::map<int, SvgCustomColor *>& customColorMap,
                      double scale, double xFocus, double yFocus) :
            Request(socketId, requestId), _mapId(mapId), _languageId(languageId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _lookIndex(lookIndex),
            _elementIds(elementIds), _customColorMap(customColorMap), _focusSetByClient(true), _scale(scale), _xFocus(xFocus), _yFocus(yFocus), _framingLevel(0),
            _createPotentialImage(false), _svgCreator(0)/*, _testMode(false)*/ {}

        ~RenderRequest();
    };
}


