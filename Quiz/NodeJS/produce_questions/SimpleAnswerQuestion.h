#pragma once

#include "ProximityCriterionTypeEnum.h"

#include <string>
#include <vector>

namespace produce_questions
{
    class Choice;
    class Point;

    class SimpleAnswerQuestion
    {
    private:
        const std::string _question;
        const std::string _answer;
        const std::string _comment;
        const std::string _questionMapId;
        const std::string _answerMapId;
        const std::string _excludedChoice;
        const ProximityCriterionTypeEnum _proximityCriterionType;
        const double _doubleCriterionValue;
        const std::string _stringCriterionValue;
        const Point * const _pointCriterionValue;
        std::vector<const Choice *> _sortedWrongChoiceVector1;
        std::vector<const Choice *> _sortedWrongChoiceVector2;

    public:
        SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment, const std::string& questionMapId, const std::string& answerMapId,
                             const std::string& excludedChoice, ProximityCriterionTypeEnum proximityCriterionType, double doubleCriterionValue, const std::string& stringCriterionValue,
                             const Point *pointCriterionValue, const std::vector<const Choice *>& choiceVector);
        ~SimpleAnswerQuestion();

        const std::string& getQuestion(void) const { return _question; }
        const std::string& getAnswer(void) const { return _answer; }
        const std::string& getComment(void) const { return _comment; }
        const std::string& getQuestionMapId(void) const { return _questionMapId; }
        const std::string& getAnswerMapId(void) const { return _answerMapId; }

        int getWrongChoiceCount1(void) const { return _sortedWrongChoiceVector1.size(); }
        const Choice *getWrongChoice1(int i) const { return _sortedWrongChoiceVector1[i]; }
        int getWrongChoiceCount2(void) const { return _sortedWrongChoiceVector2.size(); }
        const Choice *getWrongChoice2(int i) const { return _sortedWrongChoiceVector2[i]; }

    };
}

