#pragma once

#include "QuizData.h"
#include "ProximityCriterionTypeEnum.h"

namespace produce_questions
{
    class MultipleAnswerQuestion
    {
    private:
        // String
        int _question;

        // String
        int _questionMapId;

        // 3 Arrays of strings (3 arrays of same size)
        int _answerCount;
        int _answers;
        int _comments;
        int _answerMapIds;

        // String
        int _excludedChoice;

        // ProximityCriterionTypeEnum
        int _proximityCriterionType;

        // 3 doubles
        double _pointCriterionValueX;
        double _pointCriterionValueY;
        double _pointCriterionValueZ;

    public:
        static MultipleAnswerQuestion *get(int offset) { return reinterpret_cast<MultipleAnswerQuestion *>(multipleAnswerQuestions + offset); }

        const char *getQuestion(void) const { return strings + _question; }
        const char *getQuestionMapId(void) const { return strings + _questionMapId; }
        int getAnswerCount(void) const { return _answerCount; }
        const char *getAnswer(int i) const { return strings + *(intArrays + _answers + i); }
        const char *getComment(int i) const { return strings + *(intArrays + _comments + i); }
        const char *getAnswerMapId(int i) const { return strings + *(intArrays + _answerMapIds + i); }
        const char *getExcludedChoice(void) const { return strings + _excludedChoice; }
        ProximityCriterionTypeEnum getProximityCriterionType(void) const { return static_cast<ProximityCriterionTypeEnum>(_proximityCriterionType); }
        double getPointCriterionValueX(void) const { return _pointCriterionValueX; }
        double getPointCriterionValueY(void) const { return _pointCriterionValueY; }
        double getPointCriterionValueZ(void) const { return _pointCriterionValueZ; }
    };
}
