#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class MapSubParameters;

    class MapParameters
    {
    private:
        // Int
        int _framingLevel;

        // MapSubParameters
        int _questionParameters;

        // MapSubParameters
        int _answerParameters;

        // MapSubParameters
        int _wrongChoiceParameters;

        // Bool
        int _allAnswersSelectionMode;

    public:
        static MapParameters *get(int offset) { return reinterpret_cast<MapParameters *>(mapParameterss + offset); }

        int getFramingLevel(void) const { return _framingLevel; }
        const MapSubParameters *getQuestionParameters(void) const { return reinterpret_cast<MapSubParameters *>(mapSubParameterss + _questionParameters); }
        const MapSubParameters *getAnswerParameters(void) const { return reinterpret_cast<MapSubParameters *>(mapSubParameterss + _answerParameters); }
        const MapSubParameters *getWrongChoiceParameters(void) const { return reinterpret_cast<MapSubParameters *>(mapSubParameterss + _wrongChoiceParameters); }
        bool getAllAnswersSelectionMode(void) const { return _allAnswersSelectionMode != 0; }
    };
}
