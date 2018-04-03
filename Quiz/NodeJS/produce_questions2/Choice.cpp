#include "Choice.h"
#include "RandomNumberGenerator.h"
#include "QuizData.h"

namespace produce_questions
{
    double Choice::getDoubleCriterionValue(void) const
    {
        if (_doubleCriterionValueCount == 0) return 0.0;
        return *(doubleArrays + _doubleCriterionValues + RandomNumberGenerator::getRandomInt(_doubleCriterionValueCount));
    }

    const char *Choice::getStringCriterionValue(void) const
    {
        if (_stringCriterionValueCount == 0) return strings;
        return strings + *(intArrays + _stringCriterionValues + RandomNumberGenerator::getRandomInt(_stringCriterionValueCount));
    }

    const double *Choice::getPointCriterionValue(void) const
    {
        if (_pointCriterionValueCount == 0) return doubleArrays;
        return doubleArrays + _pointCriterionValues + RandomNumberGenerator::getRandomInt(_pointCriterionValueCount);
    }
}

