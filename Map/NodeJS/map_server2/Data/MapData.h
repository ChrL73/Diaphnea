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
}
