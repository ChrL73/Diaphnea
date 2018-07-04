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

#include <string>
#include <vector>

namespace map_server
{
    class RepulsiveCenter;
    class NameTranslation;
    class TextInfo;

    class MapItem
    {
    private:
        const int _itemId;
        const std::string _elementId;
        const double _importance;

        const double _xMin;
        const double _xMax;
        const double _yMin;
        const double _yMax;

        const int _textLookId;
        const char * const _textColor;
        const double _textOpacity;
        const double _textSize;

        const int _framingLevel;

        const NameTranslation * const _name;
        double _fontSize;

        std::vector<const RepulsiveCenter *> _repulsiveCenterVector;
        std::vector<TextInfo *> _textInfoVector;

    public:
        MapItem(int itemId, const std::string& elementId, double importance, double xMin, double xMax, double yMin, double yMax,
                int textLookId, const char *textColor, double textOpacity, double textSize, int framingLevel, const NameTranslation *name) :
            _itemId(itemId), _elementId(elementId), _importance(importance), _xMin(xMin), _xMax(xMax), _yMin(yMin), _yMax(yMax),
            _textLookId(textLookId), _textColor(textColor), _textOpacity(textOpacity), _textSize(textSize), _framingLevel(framingLevel), _name(name) {}

        virtual ~MapItem();

        int getItemId(void) const { return _itemId; }
        const std::string& getElementId(void) const { return _elementId; }
        double getImportance(void) const { return _importance; }

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }

        int getTextLookId(void) const { return _textLookId; }
        const char *getTextColor(void) const { return _textColor; }
        double getTextOpacity(void) const { return _textOpacity; }
        double getTextSize(void) const { return _textSize; }

        int getFramingLevel(void) const { return _framingLevel; }

        const NameTranslation *getName(void) const { return _name; }
        void setFontSize(double fontSize) { _fontSize = fontSize; }
        double getFontSize(void) const { return _fontSize; }

        virtual int getElementLookId(void) const = 0;
        virtual bool hasResolution(void) const = 0;
        virtual int getZIndex(void) const = 0;

        void addRepulsiveCenter(const RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }
        const std::vector<const RepulsiveCenter *>& getRepulsiveCenterVector(void) const { return _repulsiveCenterVector; }

        void addTextInfo(TextInfo *textInfo) { _textInfoVector.push_back(textInfo); }
        int getTextInfoCount(void) const { return _textInfoVector.size(); }
        TextInfo *getTextInfo(int i) const { return _textInfoVector[i]; }
    };
}
