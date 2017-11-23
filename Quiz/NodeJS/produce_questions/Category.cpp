#include "Category.h"
#include "MapParameters.h"

namespace produce_questions
{
    Category::~Category()
    {
        delete _mapParameters;
    }

    bool Category::isSuitable(void)
    {
        if (_suitabilityCounter == 0) return true;
        --_suitabilityCounter;
        return false;
    }
}
