#pragma once

#include "Potential.h"
#include "TextPotential.h"

#include <vector>
#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class TextDisplayerParameters;
    class TextInfo;
    class MapItem;
    class PointItem;
    class LineItem;
    class FilledPolygonItem;
    class SvgCreator;

    class TextDisplayer
    {
    private:
        static std::mutex _mutex;
        static int _counter;
        static std::map<std::string, int *> _clientActiveDisplayerMap;
        int _id;
        int *_clientActiveDisplayerId;
        bool isDisplayerActive(void);

        static std::mutex *_coutMutexPtr;

        const TextDisplayerParameters * const _parameters;
        const std::string _socketId;
        const char * const _requestId;

        const double _width;
        const double _height;
        const double _xFocus;
        const double _yFocus;
        const double _scale;
        const bool _createPotentialImage;
        SvgCreator * const _svgCreator;
        const bool _testMode;

        std::vector<MapItem *> _itemVector;

        bool displayText(MapItem *item, TextInfo *textInfo);
        bool displayPointText(PointItem *item, TextInfo *textInfo);
        bool displayLineText(LineItem *item, TextInfo *textInfo);
        bool displayFilledPolygonText(FilledPolygonItem *item, TextInfo *textInfo);

        void sendResponse(MapItem *item, TextInfo *textInfo);

        TextPotential getTextPotential(MapItem *item, TextInfo *textInfo, double x, double y, bool selfRepulsion);
        Potential getPotential(double x, double y, MapItem *selfItem);
        Potential getElementaryPotential(MapItem *item, double x, double y);

        void hsvToRgb(double h, double s, double v, double& r, double& g, double& b);

    public:
        static void setCoutMutex(std::mutex *coutMutexPtr) { _coutMutexPtr = coutMutexPtr; }
        static void clearClientMap(void);

        TextDisplayer(const TextDisplayerParameters *parameters, const std::string& socketId, const char *requestId,
            double width, double height, double xFocus, double yFocus, double scale, bool createPotentialImage, SvgCreator *svgCreator, bool testMode);
        ~TextDisplayer();

        void addItem(MapItem *item) { _itemVector.push_back(item); }
        bool start(void);
    };
}
