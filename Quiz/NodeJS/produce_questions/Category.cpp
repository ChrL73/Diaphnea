#include "Category.h"

namespace produce_questions
{
    bool Category::isSuitable(void)
    {
        if (_suitabilityCounter == 0) return true;
        --_suitabilityCounter;
        return false;
    }
}
