#include "LineItemCopy.h"
#include "Point.h"

#include <cmath>

namespace map_server
{
	LineItemCopy::~LineItemCopy()
	{
		int i, n = _pointVector.size();
		for (i = 0; i < n; ++i)
		{
			int j, m = _pointVector[i].size();
			for (j = 0; j < m; ++j)	delete _pointVector[i][j];
		}

		for (i = 0; i < _height; ++i) delete _hIntersections[i];
		delete[] _hIntersections;

		for (i = 0; i < _width; ++i) delete _vIntersections[i];
		delete[] _vIntersections;
	}

    void LineItemCopy::addPoint(double x, double y, bool newLine)
    {
        // Avoid the case where x or y is an integer (adding 0.01 pixel to x or y will not have any visible impact)
        double a;
        if (modf(y, &a) == 0.0) y += 0.01;
        if (modf(x, &a) == 0.0) x += 0.01;

        if (_pointVector.empty() || newLine) _pointVector.push_back(std::vector<const Point *>());
        _pointVector.back().push_back(new Point(x, y));
    }

    std::set<double> *LineItemCopy::getHIntersections(int y)
    {
        if (y < 0 || y >= _height) return 0;
        return _hIntersections[y];
    }

    std::set<double> *LineItemCopy::getVIntersections(int x)
    {
        if (x < 0 || x >= _width) return 0;
        return _vIntersections[x];
    }

    void LineItemCopy::setIntersections(double height, double width)
    {
        if (_hIntersections != 0) return;

        _yMin = height - 1.0;
        _yMax = 0.0;

        _height = static_cast<int>(floor(height));
        _hIntersections = new std::set<double> *[_height];
        int i;
        /*for (i = 0; i < _height; ++i) _hIntersections[i] = 0;

        int j, m = _pointVector.size();
        for (j = 0; j < m; ++j)
        {
            int n = _pointVector[j].size();
            for (i = 0; i < n - 1; ++i)
            {
                const Point *point1 = _pointVector[j][i];
                double x1 = point1->getX();
                double y1 = point1->getY();

                if (x1 > 0.0 && x1 < width)
                {
                    if (y1 < _yMin) _yMin = y1;
                    if (y1 > _yMax) _yMax = y1;
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
                    double a = (x2 - x1) / (y2- y1);
                    for (y = yMin; y <= yMax; ++y)
                    {
                        double x = x1 + a * (y - y1);
                        int yI = static_cast<int>(floor(y));
                        if (_hIntersections[yI] == 0) _hIntersections[yI] = new std::set<double>;
                        _hIntersections[yI]->insert(x);
                    }
                }
            }
        }

        if (_yMin < 0.0) _yMin = 0.0;
        if (_yMax > height - 1.0) _yMax = height - 1.0;*/

        _xMin = width - 1.0;
        _xMax = 0.0;

        _width = static_cast<int>(floor(width));
        _vIntersections = new std::set<double> *[_width];

        for (i = 0; i < _width; ++i) _vIntersections[i] = 0;

        int j, m = _pointVector.size();
        for (j = 0; j < m; ++j)
        {
            int n = _pointVector[j].size();
            for (i = 0; i < n - 1; ++i)
            {
                const Point *point1 = _pointVector[j][i];
                double x1 = point1->getX();
                double y1 = point1->getY();

                if (y1 > 0.0 && y1 < height)
                {
                    if (x1 < _xMin) _xMin = x1;
                    if (x1 > _xMax) _xMax = x1;
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
                        if (_vIntersections[xI] == 0) _vIntersections[xI] = new std::set<double>;
                        _vIntersections[xI]->insert(y);
                    }
                }
            }
        }

        if (_xMin < 0.0) _xMin = 0.0;
        if (_xMax > width - 1.0) _xMax = width - 1.0;
    }
}
