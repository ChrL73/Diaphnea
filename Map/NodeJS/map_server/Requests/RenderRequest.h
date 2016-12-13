#pragma once

#include <vector>

#include "Request.h"

namespace map_server
{
    class MapItem;

    class RenderRequest : public Request
    {
    private:
        const char * const _mapId;
        const double _widthInPixels;
        const double _heightInPixels;
        const std::vector<const char *> _elementIds;
        const bool _focusSetByClient;
        double _scale;
        double _xFocus;
        double _yFocus;

        void execute(void);
        void displayText(void);

    public:
        RenderRequest(const char *socketId, const char *requestId, const char *mapId, double widthInPixels, double heightInPixels,
                      const std::vector<const char *>& elementIds, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _elementIds(elementIds),
            _focusSetByClient(false), _scale(1.0), _xFocus(0.0), _yFocus(0.0) {}

        RenderRequest(const char *socketId, const char *requestId, const char *mapId, double widthInPixels, double heightInPixels,
                      const std::vector<const char *>& elementIds, double scale, double xFocus, double yFocus, bool sendResponse) :
            Request(socketId, requestId, sendResponse), _mapId(mapId), _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _elementIds(elementIds),
            _focusSetByClient(true), _scale(scale), _xFocus(xFocus), _yFocus(yFocus) {}
    };
}


