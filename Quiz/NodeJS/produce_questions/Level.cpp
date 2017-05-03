#include "Level.h"
#include "Category.h"

namespace produce_questions
{
    Level::~Level()
    {
        int i, n = _categoryVector.size();
        for (i = 0; i < n; ++i) delete _categoryVector[i];
    }
}
