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
        std::vector<const Choice *> _sortedWrongChoiceVector1;
        std::vector<const Choice *> _sortedWrongChoiceVector2;

    public:
        SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment, ProximityCriterionTypeEnum proximityCriterionType);

        const std::string& getQuestion(void) const { return _question; }
        const std::string& getAnswer(void) const { return _answer; }
        const std::string& getComment(void) const { return _comment; }

    };
}

