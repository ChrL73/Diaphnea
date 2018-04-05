#pragma once

#include "QuizData.h"
#include "ProximityCriterionTypeEnum.h"

namespace produce_questions
{
    class SimpleAnswerQuestion
    {
    private:
        // String
        int _question;

        // String
        int _answer;

        // String
        int _comment;

        // String
        int _questionMapId;

        // String
        int _answerMapId;

        // String
        int _excludedChoice;

        // ProximityCriterionTypeEnum
        int _proximityCriterionType;

        // String
        int _stringCriterionValue;

        // Double
        double _doubleCriterionValue;

        // 3 doubles
        double _pointCriterionValueX;
        double _pointCriterionValueY;
        double _pointCriterionValueZ;

    public:
        static SimpleAnswerQuestion *get(int offset) { return reinterpret_cast<SimpleAnswerQuestion *>(simpleAnswerQuestions + offset); }

        const char *getQuestion(void) const { return strings + _question; }
        const char *getAnswer(void) const { return strings + _answer; }
        const char *getComment(void) const { return strings + _comment; }
        const char *getQuestionMapId(void) const { return strings + _questionMapId; }
        const char *getAnswerMapId(void) const { return strings + _answerMapId; }
        const char *getExcludedChoice(void) const { return strings + _excludedChoice; }
        ProximityCriterionTypeEnum getProximityCriterionType(void) const { return static_cast<ProximityCriterionTypeEnum>(_proximityCriterionType); }
        const char *getStringCriterionValue(void) const { return strings + _stringCriterionValue; }
        double getDoubleCriterionValue(void) const { return _doubleCriterionValue; }
        double getPointCriterionValueX(void) const { return _pointCriterionValueX; }
        double getPointCriterionValueY(void) const { return _pointCriterionValueY; }
        double getPointCriterionValueZ(void) const { return _pointCriterionValueZ; }
    };
}
