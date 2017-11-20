#include "Choice.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    std::string Choice::_emptyString;

    Choice::Choice(const std::string& choiceText, const std::string& comment, const std::string& mapId, const std::vector<double>& doubleCriterionVector,
    const std::vector<std::string>& stringCriterionVector, const std::vector<Point>& pointCriterionVector) :
            _choiceText(choiceText), _comment(comment), _mapId(mapId),
            _doubleCriterionValue(doubleCriterionVector.empty() ? 0 : doubleCriterionVector[RandomNumberGenerator::getRandomInt(doubleCriterionVector.size())]),
            _stringCriterionValue(stringCriterionVector.empty() ? _emptyString : stringCriterionVector[RandomNumberGenerator::getRandomInt(stringCriterionVector.size())]),
            _pointCriterionValue(pointCriterionVector.empty() ? Point(0.0, 0.0, 0.0) : pointCriterionVector[RandomNumberGenerator::getRandomInt(pointCriterionVector.size())])
    {
    }
}
