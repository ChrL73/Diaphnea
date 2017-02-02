#pragma once

#include <map>
#include <string>

namespace map_server
{
    class SvgInfo;

    class SvgCreator
    {
    private:
        const double _widthInPixels;
        const double _heightInPixels;
        const double _scale;
        const double _sizeFactor;
        const double _xFocus;
        const double _yFocus;
        const std::string _socketId;

        std::multimap<int, SvgInfo *> _infoMap;

    public:
        SvgCreator(double widthInPixels, double heightInPixels, double scale, double sizeFactor, double xFocus, double yFocus, const std::string& socketId) :
            _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _scale(scale), _sizeFactor(sizeFactor), _xFocus(xFocus), _yFocus(yFocus),_socketId(socketId) {}
        ~SvgCreator();

        void addInfo(int zIndex, SvgInfo *info) { _infoMap.insert(std::pair<int, SvgInfo *>(-zIndex, info)); }
        void execute(void);

    };
}
