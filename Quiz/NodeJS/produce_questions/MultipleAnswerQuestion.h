#pragma once

#include "ProximityCriterionTypeEnum.h"

#include <set>
#include <string>
#include <vector>

namespace produce_questions
{
    class Choice;
    class Point;
    class TextAndComment;

    class MultipleAnswerQuestion
    {
    private:
        const std::string _question;
        const std::string _questionMapId;
        const std::vector<const TextAndComment *> _answerVector;
        const std::vector<std::string> _answerMapIds;
        const std::string _excludedChoice;
        const ProximityCriterionTypeEnum _proximityCriterionType;
        const Point * const _pointCriterionValue;
        std::vector<const Choice *> _sortedWrongChoiceVector;

    public:
        MultipleAnswerQuestion(const std::string& question, const std::string& questionMapId, const std::vector<const TextAndComment *>& answerVector,
                               const std::vector<std::string>& answerMapIds, const std::string& excludedChoice, ProximityCriterionTypeEnum proximityCriterionType,
                               const Point *pointCriterionValue, const std::vector<const Choice *>& choiceVector);
        ~MultipleAnswerQuestion();

        const std::string& getQuestion(void) const { return _question; }
        const std::string& getQuestionMapId(void) const { return _questionMapId; }
        int getAnswerCount(void) const { return _answerVector.size(); }
        const TextAndComment *getAnswer(int i) const { return _answerVector[i]; }
        const std::string& getAnswerMapId(int i) const { return _answerMapIds[i]; }
        int getWrongChoiceCount(void) const { return _sortedWrongChoiceVector.size(); }
        const Choice *getWrongChoice(int i) const { return _sortedWrongChoiceVector[i]; }
    };
}
