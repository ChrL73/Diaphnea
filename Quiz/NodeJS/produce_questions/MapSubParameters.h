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

    public:
        MapSubParameters(int drawDepth, const std::string& categorySelectionMode, const std::vector<int>& categories) :
            _drawDepth(drawDepth), _categorySelectionMode(categorySelectionMode), _categories(categories) {}

        int getDrawDepth(void) const { return _drawDepth; }
        const std::string& getCategorySelectionMode(void) const { return _categorySelectionMode; }
        int getCategoryCount(void) const { return _categories.size(); }
        int getCategory(int i) const { return _categories[i]; }
    };
}
