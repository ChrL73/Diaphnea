#pragma once

#include "QuizData.h"

namespace produce_questions
{
    class SimpleAnswerCategory;
    class MultipleAnswerCategory;
    class RelationOrderCategory;
    class AttributeOrderCategory;

    class Level
    {
    private:
        // Array of SimpleAnswerCategory
        int _simpleAnswerCategoryCount;
        int _simpleAnswerCategories;

        // Array of MultipleAnswerCategory
        int _multipleAnswerCategoryCount;
        int _multipleAnswerCategories;

        // Array of RelationOrderCategory
        int _relationOrderCategoryCount;
        int _relationOrderCategories;

        // Array of AttributeOrderCategory
        int _attributeOrderCategoryCount;
        int _attributeOrderCategories;

        // Int
        int _questionCount;

        // Int
        int _weightSum;

        // Double
        double _distribParameter;

        // Int
        int _choiceCount;

    public:
        static const Level *instance(void) { return reinterpret_cast<const Level *>(level); }

        int getSimpleAnswerCategoryCount(void) const { return _simpleAnswerCategoryCount; }
        const SimpleAnswerCategory *getSimpleAnswerCategory(int i) const
        {
            return reinterpret_cast<const SimpleAnswerCategory *>(simpleAnswerCategories + *(intArrays + _simpleAnswerCategories + i));
        }

        int getMultipleAnswerCategoryCount(void) const { return _multipleAnswerCategoryCount; }
        const MultipleAnswerCategory *getMultipleAnswerCategory(int i) const
        {
            return reinterpret_cast<const MultipleAnswerCategory *>(multipleAnswerCategories + *(intArrays + _multipleAnswerCategories + i));
        }

        int getRelationOrderCategoryCount(void) const { return _relationOrderCategoryCount; }
        const RelationOrderCategory *getRelationOrderCategory(int i) const
        {
            return reinterpret_cast<const RelationOrderCategory *>(relationOrderCategories + *(intArrays + _relationOrderCategories + i));
        }

        int getAttributeOrderCategoryCount(void) const { return _attributeOrderCategoryCount; }
        const AttributeOrderCategory *getAttributeOrderCategory(int i) const
        {
            return reinterpret_cast<const AttributeOrderCategory *>(attributeOrderCategories + *(intArrays + _attributeOrderCategories + i));
        }

        int getQuestionCount(void) const { return _questionCount; }
        int getWeightSum(void) const { return _weightSum; }
        double getDistribParameter(void) const { return _distribParameter; }
        int getChoiceCount(void) const { return _choiceCount; }
    };
}
