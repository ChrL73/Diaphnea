#include "Choice.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    Choice::Choice(const std::string& choiceText, const std::string& comment, const std::vector<double>& doubleCriterionVector, const std::vector<std::string>& stringCriterionVector) :
            _choiceText(choiceText), _comment(comment),
            _doubleCriterionValue(doubleCriterionVector.empty() ? 0 : doubleCriterionVector[RandomNumberGenerator::getRandomInt(doubleCriterionVector.size())]),
            _stringCriterionValue(stringCriterionVector.empty() ? _emptyString : stringCriterionVector[RandomNumberGenerator::getRandomInt(stringCriterionVector.size())])
    {
    }
}
