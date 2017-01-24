#pragma once

#include "Potential.h"

#include <vector>
#include <map>
#include <string>
#include <mutex>

namespace map_server
{
    class ItemCopy;
    class PointItemCopy;
    class LineItemCopy;
    class FilledPolygonItemCopy;
	class TextDisplayerParameters;
	class TextInfo;

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
		const char * const _socketId;
        const char * const _requestId;

        const double _width;
        const double _height;
        const double _xFocus;
        const double _yFocus;
        const double _scale;
        const bool _createPotentialImage;

        std::vector<ItemCopy *> _itemVector;

        bool displayText(ItemCopy *item, TextInfo *textInfo);
        bool displayPointText(PointItemCopy *item, TextInfo *textInfo);
        bool displayLineText(LineItemCopy *item, TextInfo *textInfo);
        bool displayFilledPolygonText(FilledPolygonItemCopy *item, TextInfo *textInfo);

        void sendResponse(ItemCopy *item, TextInfo *textInfo);

        double getTextPotential(ItemCopy *item, TextInfo *textInfo, double x, double y, bool selfRepulsion);
        Potential getPotential(double x, double y, ItemCopy *selfItem);
        Potential getElementaryPotential(ItemCopy *item, double x, double y);

        void hsvToRgb(double h, double s, double v, double& r, double& g, double& b);

    public:
        static void setCoutMutex(std::mutex *coutMutexPtr) { _coutMutexPtr = coutMutexPtr; }
		static void clearClientMap(void);

        TextDisplayer(const TextDisplayerParameters *parameters, const char *socketId, const char *requestId,
            double width, double height, double xFocus, double yFocus, double scale, bool createPotentialImage);
		~TextDisplayer();

        void addItem(ItemCopy *item) { _itemVector.push_back(item); }
        void start(void);
    };
}
