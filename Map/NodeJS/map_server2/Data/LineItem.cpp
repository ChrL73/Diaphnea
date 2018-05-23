#include "LineItem.h"
#include "MultipointItem.h"
#include "LineLook.h"
#include "PolygonLook.h"
#include "LineElement.h"
#include "Point.h"

#include <cmath>

namespace map_server
{
    LineItem::LineItem(const LineElement *lineElement, const NameTranslation *name, const MultipointItem *multipointItem, const LineLook *lineLook) :
        MapItem(multipointItem->getItemId(), lineElement->getElementId(), lineElement->getImportance(), multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(), 3 * lineLook->getLookId(),
        lineLook->getTextColor(), lineLook->getLineOpacity(), lineLook->getTextSize(), lineElement->getFramingLevel(), name),
        _lineLookId(3 * lineLook->getLookId() + 1), _lineZIndex(lineLook->getLineZIndex()),
        _lineColor(lineLook->getLineColor()), _lineOpacity(lineLook->getLineOpacity()),
        _lineSize(lineLook->getLineSize()), _multipointItem(multipointItem),
        _hIntersections(0), _vIntersections(0)
    {
    }

    LineItem::LineItem(const MultipointItem *multipointItem, const PolygonLook *polygonLook) :
        MapItem(multipointItem->getItemId(), "", -1.0, multipointItem->getXMin(), multipointItem->getXMax(),
        multipointItem->getYMin(), multipointItem->getYMax(),-1, 0, 0.0, 0.0, -1, 0),
        _lineLookId(3 * polygonLook->getLookId() + 1), _lineZIndex(polygonLook->getContourZIndex()),
        _lineColor(polygonLook->getContourColor()), _lineOpacity(polygonLook->getContourOpacity()),
        _lineSize(polygonLook->getContourSize()), _multipointItem(multipointItem),
        _hIntersections(0), _vIntersections(0)
    {
    }

    LineItem::~LineItem()
    {
        int i, n = _pointVector.size();
        for (i = 0; i < n; ++i)
        {
            int j, m = _pointVector[i].size();
            for (j = 0; j < m; ++j) delete _pointVector[i][j];
        }

        for (i = 0; i < _height; ++i) delete _hIntersections[i];
        delete[] _hIntersections;

        for (i = 0; i < _width; ++i) delete _vIntersections[i];
        delete[] _vIntersections;
    }

    const PointList *LineItem::getPointList(int resolutionIndex) const
    {
        return _multipointItem->getPointList(resolutionIndex);
    }

    bool LineItem::cap1Round(void) const
    {
        return _multipointItem->cap1Round();
    }

    bool LineItem::cap2Round(void) const
    {
        return _multipointItem->cap2Round();
    }

    void LineItem::updateLook(const PolygonLook *polygonLook)
    {
        if (polygonLook->getContourZIndex() < _lineZIndex)
        {
            _lineZIndex = polygonLook->getContourZIndex();
            _lineColor = polygonLook->getContourColor();
            _lineOpacity = polygonLook->getContourOpacity();
            _lineSize = polygonLook->getContourSize();
            _lineLookId = 3 * polygonLook->getLookId() + 1;
        }
    }

    void LineItem::addPoint(double x, double y, bool newLine)
    {
        // Avoid the case where x or y is an integer (adding 0.01 pixel to x or y will not have any visible impact)
        double a;
        if (modf(y, &a) == 0.0) y += 0.01;
        if (modf(x, &a) == 0.0) x += 0.01;

        if (_pointVector.empty() || newLine) _pointVector.push_back(std::vector<const Point *>());
        _pointVector.back().push_back(new Point(x, y));
    }

    std::map<double, double> *LineItem::getHIntersections(int y)
    {
        if (y < 0 || y >= _height) return 0;
        return _hIntersections[y];
    }

    std::map<double, double> *LineItem::getVIntersections(int x)
    {
        if (x < 0 || x >= _width) return 0;
        return _vIntersections[x];
    }

    void LineItem::setIntersections(double height, double width)
    {
        if (_hIntersections != 0) return;

        _yMinP = height - 1.0;
        _yMaxP = 0.0;

        _height = static_cast<int>(floor(height));
        _hIntersections = new std::map<double, double> *[_height];
        int i;
        for (i = 0; i < _height; ++i) _hIntersections[i] = 0;

        int j, m = _pointVector.size();
        for (j = 0; j < m; ++j)
        {
            std::vector<std::pair<int, double> > intersectionVector;

            int n = _pointVector[j].size();
            for (i = 0; i < n - 1; ++i)
            {
                const Point *point1 = _pointVector[j][i];
                double x1 = point1->getX();
                double y1 = point1->getY();

                if (x1 > 0.0 && x1 < width)
                {
                    if (y1 < _yMinP) _yMinP = y1;
                    if (y1 > _yMaxP) _yMaxP = y1;
                }

                const Point *point2;
                point2 = _pointVector[j][i + 1];
                double x2 = point2->getX();
                double y2 = point2->getY();

                double yMin, yMax;
                if (y1 > y2)
                {
                    yMin = ceil(y2);
                    yMax = floor(y1);
                }
                else
                {
                    yMin = ceil(y1);
                    yMax = floor(y2);
                }

                if (yMin <= yMax && yMax >= 0.0 && yMin <= height - 1.0)
                {
                    if (yMin < 0.0 ) yMin = 0.0;
                    if (yMax > _height - 1.0) yMax = _height - 1.0;

                    double y;
                    double a = (x2 - x1) / (y2 - y1);
                    for (y = yMin; y <= yMax; ++y)
                    {
                        double x = x1 + a * (y - y1);
                        int yI = static_cast<int>(floor(y));
                        intersectionVector.push_back(std::pair<int, double>(yI, x));
                    }
                }
            }

            n = intersectionVector.size();
            for (i = 0; i < n; ++i)
            {
                int yI = intersectionVector[i].first;
                double x = intersectionVector[i].second;

                double slope1 = 0.0;
                double slope2 = 0.0;
                if (i != 0) slope1 = (x - intersectionVector[i - 1].second) / static_cast<double>(yI - intersectionVector[i - 1].first);
                if (i != n - 1) slope2 = (intersectionVector[i + 1].second - x) / static_cast<double>(intersectionVector[i + 1].first - yI);
                if (slope1 < 0) slope1 = -slope1;
                if (slope2 < 0) slope2 = -slope2;
                double slope = slope1 > slope2 ? slope1 : slope2;

                if (_hIntersections[yI] == 0) _hIntersections[yI] = new std::map<double, double>;
                _hIntersections[yI]->insert(std::pair<double, double>(x, 0.5 * slope));
            }
        }

        if (_yMinP < 0.0) _yMinP = 0.0;
        if (_yMaxP > height - 1.0) _yMaxP = height - 1.0;

        _xMinP = width - 1.0;
        _xMaxP = 0.0;

        _width = static_cast<int>(floor(width));
        _vIntersections = new std::map<double, double> *[_width];

        for (i = 0; i < _width; ++i) _vIntersections[i] = 0;

        m = _pointVector.size();
        for (j = 0; j < m; ++j)
        {
            std::vector<std::pair<int, double> > intersectionVector;

            int n = _pointVector[j].size();
            for (i = 0; i < n - 1; ++i)
            {
                const Point *point1 = _pointVector[j][i];
                double x1 = point1->getX();
                double y1 = point1->getY();

                if (y1 > 0.0 && y1 < height)
                {
                    if (x1 < _xMinP) _xMinP = x1;
                    if (x1 > _xMaxP) _xMaxP = x1;
                }

                const Point *point2;
                point2 = _pointVector[j][i + 1];
                double x2 = point2->getX();
                double y2 = point2->getY();

                double xMin, xMax;
                if (x1 > x2)
                {
                    xMin = ceil(x2);
                    xMax = floor(x1);
                }
                else
                {
                    xMin = ceil(x1);
                    xMax = floor(x2);
                }

                if (xMin <= xMax && xMax >= 0.0 && xMin <= width - 1.0)
                {
                    if (xMin < 0.0 ) xMin = 0.0;
                    if (xMax > _width - 1.0) xMax = _width - 1.0;

                    double x;
                    double a = (y2 - y1) / (x2- x1);
                    for (x = xMin; x <= xMax; ++x)
                    {
                        double y = y1 + a * (x - x1);
                        int xI = static_cast<int>(floor(x));
                        intersectionVector.push_back(std::pair<int, double>(xI, y));
                    }
                }
            }

            n = intersectionVector.size();
            for (i = 0; i < n; ++i)
            {
                int xI = intersectionVector[i].first;
                double y = intersectionVector[i].second;

                double slope1 = 0.0;
                double slope2 = 0.0;
                if (i != 0) slope1 = (y - intersectionVector[i - 1].second) / static_cast<double>(xI - intersectionVector[i - 1].first);
                if (i != n - 1) slope2 = (intersectionVector[i + 1].second - y) / static_cast<double>(intersectionVector[i + 1].first - xI);
                if (slope1 < 0) slope1 = -slope1;
                if (slope2 < 0) slope2 = -slope2;
                double slope = slope1 > slope2 ? slope1 : slope2;

                if (_vIntersections[xI] == 0) _vIntersections[xI] = new std::map<double, double>;
                _vIntersections[xI]->insert(std::pair<double, double>(y, 0.5 * slope));
            }
        }

        if (_xMinP < 0.0) _xMinP = 0.0;
        if (_xMaxP > width - 1.0) _xMaxP = width - 1.0;
    }
}
