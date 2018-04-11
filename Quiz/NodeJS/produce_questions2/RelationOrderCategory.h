#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class MapParameters;
    class RelationOrderQuestion;
    class CompleteQuestion;

    class RelationOrderCategory
    {
        private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // Array of RelationOrderQuestion
        int _questionCount;
        int _questions;

        // Double
        double _distribParameterCorrection;

    public:
        static RelationOrderCategory *get(int offset) { return reinterpret_cast<RelationOrderCategory *>(relationOrderCategories + offset); }

        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        int getQuestionCount(void) const { return _questionCount; }
        const RelationOrderQuestion *getQuestion(int i) const { return reinterpret_cast<const RelationOrderQuestion *>(relationOrderQuestions + *(intArrays + _questions + i)); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;
    };
}
