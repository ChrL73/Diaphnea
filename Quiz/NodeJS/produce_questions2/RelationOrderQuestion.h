#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class CompleteQuestion;
    class MapParameters;

    class RelationOrderQuestion
    {
    private:
        // String
        int _question;

        // String
        int _mapId;

        // 2 Arrays of strings (2 arrays of same size)
        int _choiceCount;
        int _choiceTexts;
        int _choiceMapIds;

    public:
        static RelationOrderQuestion *get(int offset) { return reinterpret_cast<RelationOrderQuestion *>(relationOrderQuestions + offset); }

        const char *getQuestion(void) const { return strings + _question; }
        const char *getMapId(void) const { return strings + _mapId; }

        int getChoiceCount(void) const { return _choiceCount; }
        const char *getChoiceText(int i) const { return strings + *(intArrays + _choiceTexts + i); }
        const char *getChoiceMapId(int i) const { return strings + *(intArrays + _choiceMapIds + i); }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter, const MapParameters *mapParameters) const;
    };
}
