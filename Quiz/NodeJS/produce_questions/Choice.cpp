#include "Choice.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    double Choice::getDoubleCriterionValue(void) const
    {
        int n = _doubleCriterionVector.size();
        if (n == 0) return 0.0;

        int draw = RandomNumberGenerator::getRandomInt(n);
        return _doubleCriterionVector[draw];
    }

    const std::string& Choice::getStringCriterionValue(void) const
    {
        int n = _stringCriterionVector.size();
        if (n == 0) return _emptyString;

        int draw = RandomNumberGenerator::getRandomInt(n);
        return _stringCriterionVector[draw];
    }
}
