#pragma once

#include "Category.h"
#include "ProximityCriterionTypeEnum.h"

#include <string>

namespace produce_questions
{
    class SimpleAnswerCategory : public Category
    {
    private:
        const int _questionCount;
        const std::string _questionListId;
        const int _choiceCount;
        const std::string _choiceListId;
        const double _distribParameterCorrection;
        const ProximityCriterionTypeEnum _proximityCriterionType;

        const CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;

    public:
        SimpleAnswerCategory(unsigned int weightIndex, int questionCount, const std::string& questionListId, int choiceCount, const std::string& choiceListId,
                             double _distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType) :
            Category(weightIndex), _questionCount(questionCount), _questionListId(questionListId), _choiceCount(choiceCount), _choiceListId(choiceListId),
            _distribParameterCorrection(_distribParameterCorrection), _proximityCriterionType(proximityCriterionType) {}
    };
}
