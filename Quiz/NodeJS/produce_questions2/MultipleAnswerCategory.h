#pragma once

#include "QuizData.h"
#include "ProximityCriterionTypeEnum.h"

namespace produce_questions
{
    class MapParameters;
    class MultipleAnswerQuestion;
    class Choice;
    class CompleteQuestion;

    class MultipleAnswerCategory
    {
    private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // Array of MultipleAnswerQuestion
        int _questionCount;
        int _questions;

        // Array of Chocie
        int _choiceCount;
        int _choices;

        // Double
        double _distribParameterCorrection;

        // ProximityCriterionTypeEnum
        int _proximityCriterionType;

    public:
        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        int getQuestionCount(void) const { return _questionCount; }
        const MultipleAnswerQuestion *getQuestion(int i) const { return reinterpret_cast<const MultipleAnswerQuestion *>(multipleAnswerQuestions + *(intArrays + _questions + i)); }

        int getChoiceCount(void) const { return _choiceCount; }
        const Choice *getChoice(int i) const { return reinterpret_cast<const Choice *>(choices + *(intArrays + _choices + i)); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        ProximityCriterionTypeEnum getProximityCriterionType(void) const { return static_cast<ProximityCriterionTypeEnum>(_proximityCriterionType); }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;
    };
}

