#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class MapSubParameters
    {
    private:
        // Int
        int _drawDepth;

        // String
        int _categorySelectionMode;

        // Array of ints
        int _categoryCount;
        int _categories;

        // int
        int _framingMode;

    public:
        //static MapSubParameters *get(int offset) { return reinterpret_cast<MapSubParameters *>(mapSubParameterss + offset); }

        int getDrawDepth(void) const { return _drawDepth; }
        const char *getCategorySelectionMode(void) const { return strings + _categorySelectionMode; }
        int getCategoryCount(void) const { return _categoryCount; }
        int getCategory(int i) const { return *(intArrays + _categories + i); }
        int getFramingMode(void) const { return _framingMode; }
    };
}
