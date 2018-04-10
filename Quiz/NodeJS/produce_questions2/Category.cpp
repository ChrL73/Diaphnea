#include "Category.h"
#include "SimpleAnswerCategory.h"
#include "MultipleAnswerCategory.h"
#include "RelationOrderCategory.h"
//#include "AttributeOrderCategory.h"

namespace produce_questions
{
    bool Category::isSuitable(void)
    {
        if (_suitabilityCounter == 0) return true;
        --_suitabilityCounter;
        return false;
    }

    unsigned int Category::getWeightIndex(void) const
    {
        if (_simpleAnswerCategory != 0) return _simpleAnswerCategory->getWeightIndex();
        if (_multipleAnswerCategory != 0) return _multipleAnswerCategory->getWeightIndex();
        if (_relationOrderCategory != 0) return _relationOrderCategory->getWeightIndex();
        //if (_attributeOrderCategory != 0) return _attributeOrderCategory->getWeightIndex();

        return 0;
    }

    CompleteQuestion *Category::getNewQuestion(int choiceCount, double distribParameter)
    {
        if (_simpleAnswerCategory != 0) return _simpleAnswerCategory->getNewQuestion(choiceCount, distribParameter);
        if (_multipleAnswerCategory != 0) return _multipleAnswerCategory->getNewQuestion(choiceCount, distribParameter);
        if (_relationOrderCategory != 0) return _relationOrderCategory->getNewQuestion(choiceCount, distribParameter);
        //if (_attributeOrderCategory != 0) return _attributeOrderCategory->getNewQuestion(choiceCount, distribParameter);

        return 0;
    }
}
