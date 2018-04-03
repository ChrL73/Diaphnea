#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class Choice
    {
    private:
        int _choiceText;
        int _comment;
        int _mapId;
        int _doubleCriterionValueCount;
        int _doubleCriterionValues;
        int _stringCriterionValueCount;
        int _stringCriterionValues;
        int _pointCriterionValueCount;
        int _pointCriterionValues;

    public:
        static Choice *get(int offset) { return reinterpret_cast<Choice *>(choices + offset); }

        const char *getChoiceText(void) const { return strings + _choiceText; }
        const char *getComment(void) const { return strings + _comment; }
        const char *getMapId(void) const { return strings + _mapId; }
        double getDoubleCriterionValue(void) const;
        const char *getStringCriterionValue(void) const;
        const double *getPointCriterionValue(void) const;
    };
}
