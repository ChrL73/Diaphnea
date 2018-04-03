#pragma once

#include "Point.h"

#include <string>
#include <vector>

namespace produce_questions
{
    class Choice
    {
    private:
        const std::string _choiceText;
        const std::string _comment;
        const std::string _mapId;
        const double _doubleCriterionValue;
        const std::string _stringCriterionValue;
        const Point _pointCriterionValue;

        static std::string _emptyString;

    public:
        Choice(const std::string& choiceText, const std::string& comment, const std::string& mapId, const std::vector<double>& doubleCriterionVector,
               const std::vector<std::string>& stringCriterionVector, const std::vector<Point>& pointCriterionVector);

        const std::string& getChoiceText(void) const { return _choiceText; }
        const std::string& getComment(void) const { return _comment; }
        const std::string& getMapId(void) const { return _mapId; }
        double getDoubleCriterionValue(void) const { return _doubleCriterionValue; }
        const std::string& getStringCriterionValue(void) const { return _stringCriterionValue; }
        const Point& getPointCriterionValue(void) const { return _pointCriterionValue; }
    };
}
