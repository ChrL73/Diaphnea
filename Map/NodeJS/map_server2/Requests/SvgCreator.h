/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#pragma once

#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class SvgInfo;
    class SvgCustomColor;

    class SvgCreator
    {
    private:
        static std::mutex *_coutMutexPtr;

        const double _widthInPixels;
        const double _heightInPixels;
        const double _scale;
        const double _sizeFactor;
        const double _xFocus;
        const double _yFocus;
        const std::string _socketId;
        const std::string _requestId;
        const std::map<int, SvgCustomColor *> *_customColorMap;

        std::multimap<int, SvgInfo *> _infoMap;

    public:
        SvgCreator(double widthInPixels, double heightInPixels, double scale, double sizeFactor, double xFocus, double yFocus,
                   const std::string& socketId, const std::string& requestId, const std::map<int, SvgCustomColor *> *customColorMap) :
            _widthInPixels(widthInPixels), _heightInPixels(heightInPixels), _scale(scale), _sizeFactor(sizeFactor), _xFocus(xFocus), _yFocus(yFocus),
            _socketId(socketId), _requestId(requestId), _customColorMap(customColorMap) {}
        ~SvgCreator();

        static void setCoutMutex(std::mutex *coutMutexPtr) { _coutMutexPtr = coutMutexPtr; }

        void addInfo(int zIndex, SvgInfo *info) { _infoMap.insert(std::pair<int, SvgInfo *>(-zIndex, info)); }
        void execute(void);
    };
}
