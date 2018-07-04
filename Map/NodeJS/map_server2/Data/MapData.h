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

namespace map_server
{
    typedef struct { int a; int b; double *c; } pList;

    extern unsigned char strings[];
    extern int intArrays[];
    extern int pointElements[];
    extern int lineElements[];
    extern int polygonElements[];
    extern int multipointItems[];
    extern int pointLooks[];
    extern int lineLooks[];
    extern int polygonLooks[];
    extern int pointElementCount;
    extern int lineElementCount;
    extern int polygonElementCount;
    extern double zoomMinDistance;
    extern double zoomMaxDistance;
    extern double resolutionThreshold;
    extern double sizeParameter1;
    extern double sizeParameter2;
    extern int sampleLengthCount;
    extern double sampleLengths[];
    extern int languageCount;
    extern int languageIds[];
    extern int nameTranslations[];
    extern int elementNames[];
    extern pList pointLists[];
    extern int mapInfo;
    extern int itemCount;
    extern unsigned char *itemData[];
}
