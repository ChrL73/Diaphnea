#pragma once

namespace produce_questions
{
    class SimpleAnswerCategory;
    class MultipleAnswerCategory;
    class RelationOrderCategory;
    class AttributeOrderCategory;
    class CompleteQuestion;

    class Category
    {
    private:
        const SimpleAnswerCategory * const _simpleAnswerCategory;
        const MultipleAnswerCategory * const _multipleAnswerCategory;
        const RelationOrderCategory * const _relationOrderCategory;
        const AttributeOrderCategory * const _attributeOrderCategory;
        int _suitabilityCounter;

    public:
        Category(const SimpleAnswerCategory *simpleAnswerCategory) :
            _simpleAnswerCategory(simpleAnswerCategory), _multipleAnswerCategory(0), _relationOrderCategory(0),
            _attributeOrderCategory(0), _suitabilityCounter(0) {}

        Category(const MultipleAnswerCategory *multipleAnswerCategory) :
            _simpleAnswerCategory(0), _multipleAnswerCategory(multipleAnswerCategory), _relationOrderCategory(0),
            _attributeOrderCategory(0), _suitabilityCounter(0) {}

        Category(const RelationOrderCategory *relationOrderCategory) :
            _simpleAnswerCategory(0), _multipleAnswerCategory(0), _relationOrderCategory(relationOrderCategory),
            _attributeOrderCategory(0), _suitabilityCounter(0) {}

        Category(const AttributeOrderCategory *attributeOrderCategory) :
            _simpleAnswerCategory(0), _multipleAnswerCategory(0), _relationOrderCategory(0),
            _attributeOrderCategory(attributeOrderCategory), _suitabilityCounter(0) {}

        void reduceSuitability(int i) { _suitabilityCounter += i; }
        void increaseSuitability(void) { if (_suitabilityCounter > 0) --_suitabilityCounter; }
        bool isSuitable(void);

        unsigned int getWeightIndex(void) const;

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter);
    };
}
