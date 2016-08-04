#pragma once

#include "ProximityCriterionTypeEnum.h"

#include <string>
#include <vector>

namespace produce_questions
{
    class Choice;

    class SimpleAnswerQuestion
    {
    private:
        const std::string _question;
        const std::string _answer;
        const std::string _comment;
        const ProximityCriterionTypeEnum _proximityCriterionType;
        const double _doubleCriterionValue;
        const std::string _stringCriterionValue;
        std::vector<const Choice *> _sortedWrongChoiceVector1;
        std::vector<const Choice *> _sortedWrongChoiceVector2;

    public:
        SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment, ProximityCriterionTypeEnum proximityCriterionType,
                             double doubleCriterionValue, const std::string& stringCriterionValue, const std::vector<const Choice *>& choiceVector);

        const std::string& getQuestion(void) const { return _question; }
        const std::string& getAnswer(void) const { return _answer; }
        const std::string& getComment(void) const { return _comment; }

        int getWrongChoiceCount1(void) const { return _sortedWrongChoiceVector1.size(); }
        const Choice *getWrongChoice1(int i) const { return _sortedWrongChoiceVector1[i]; }
        int getWrongChoiceCount2(void) const { return _sortedWrongChoiceVector2.size(); }
        const Choice *getWrongChoice2(int i) const { return _sortedWrongChoiceVector2[i]; }

    };
}

