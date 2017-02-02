#pragma once

#include <vector>

#include "Request.h"

typedef struct FT_FaceRec_ *FT_Face;

namespace map_server
{
    class ItemCopyBuilder;
    class ItemCopy;
    class Map;
    class SvgCreator;

    class RenderRequest : public Request
    {
    private:
        const char * const _mapId;
        const char * const _languageId;
        const double _widthInPixels;
        const double _heightInPixels;
        const std::vector<const char *> _elementIds;
        const bool _focusSetByClient;
        double _scale;
        double _xFocus;
        double _yFocus;
        bool _createPotentialImage;
        SvgCreator *_svgCreator;

        Map *_map;
        std::vector<ItemCopyBuilder *> _itemCopyBuilderVector;

        void execute(void);
        bool displayText(void);
        void setTextInfo(ItemCopy *itemCopy, ItemCopyBuilder *itemCopyBuilder, double sizeFactor, FT_Face face);

    public:
        RenderRequest(const char *socketId, const char *requestId, const char *mapId, const char *languageId, double widthInPixels, double heightInPixels,
                      const std::vector<const char *>& elementIds, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _languageId(languageId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _elementIds(elementIds),
            _focusSetByClient(false), _scale(1.0), _xFocus(0.0), _yFocus(0.0), _createPotentialImage(false), _svgCreator(0), _map(0) {}

        RenderRequest(const char *socketId, const char *requestId, const char *mapId, const char *languageId, double widthInPixels, double heightInPixels,
                      const std::vector<const char *>& elementIds, double scale, double xFocus, double yFocus, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _languageId(languageId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _elementIds(elementIds),
            _focusSetByClient(true), _scale(scale), _xFocus(xFocus), _yFocus(yFocus), _createPotentialImage(false), _svgCreator(0), _map(0) {}
    };
}


