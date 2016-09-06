#pragma once

#include "Category.h"

#include <string>
#include <vector>

namespace produce_questions
{
    class AttributeOrderChoice;

    class AttributeOrderCategory : public Category
    {
    private:
        const std::string _questionText;
        const int _choiceCount;
        const std::string _choiceListId;
        const double _distribParameterCorrection;
        const int _maxIndex;

        std::vector<const AttributeOrderChoice *> _choiceVector;

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;

    public:
        AttributeOrderCategory(unsigned int weightIndex, const std::string& questionText, int choiceCount, const std::string& choiceListId, double distribParameterCorrection, int maxIndex);
    };
}
