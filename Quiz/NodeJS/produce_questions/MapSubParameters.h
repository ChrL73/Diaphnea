#pragma once

#include <vector>
#include <string>

namespace produce_questions
{
    class MapSubParameters
    {
    private:
        const int _drawDepth;
        const std::string _categorySelectionMode;
        const std::vector<int> _categories;
        const int _framingMode;

    public:
        MapSubParameters(int drawDepth, const std::string& categorySelectionMode, const std::vector<int>& categories, int framingMode) :
            _drawDepth(drawDepth), _categorySelectionMode(categorySelectionMode), _categories(categories), _framingMode(framingMode) {}

        int getDrawDepth(void) const { return _drawDepth; }
        const std::string& getCategorySelectionMode(void) const { return _categorySelectionMode; }
        int getCategoryCount(void) const { return _categories.size(); }
        int getCategory(int i) const { return _categories[i]; }
        int getFramingMode(void) const { return _framingMode; }
    };
}
