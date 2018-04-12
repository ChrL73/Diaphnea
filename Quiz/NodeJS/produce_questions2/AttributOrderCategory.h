#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class MapParameters;

    class AttributeOrderCategory
    {
    private:
        // Int
        int _weightIndex;

        // MapParameters
        int _mapParameters;

        // String
        int _questionText;

        //  4 arrays of same size
        int _choiceCount;
        int _choiceTexts; // Array of strings
        int _choiceComments; // Array of strings
        int _choiceMinIndexes; // Array of ints
        int _choiceMapIds; // Array of strings

        // Double
        double _distribParameterCorrection;

        // Int
        int _maxIndex;

    public:
        static AttributeOrderCategory *get(int offset) { return reinterpret_cast<AttributeOrderCategory *>(attributeOrderCategories + offset); }

        unsigned int getWeightIndex(void) const { return _weightIndex; }

        const char *getQuestionText(void) const { return strings + _questionText; }

        const MapParameters *getMapParameters(void) const { return reinterpret_cast<MapParameters *>(mapParameterss + _mapParameters); }

        int getChoiceCount(void) const { return _choiceCount; }
        const char *getChoiceText(int i) const { return strings + *(intArrays + _choiceTexts + i); }
        const char *getChoiceComment(int i) const { return strings + *(intArrays + _choiceComments + i); }
        int getChoiceMinIndex(int i) const { return *(intArrays + _choiceMinIndexes + i); }
        const char *getChoiceMapId(int i) const { return strings + *(intArrays + _choiceMapIds + i); }

        double getDistribParameterCorrection(void) const { return _distribParameterCorrection; }

        int getMaxIndex(void) const { return _maxIndex; }

    };
}
