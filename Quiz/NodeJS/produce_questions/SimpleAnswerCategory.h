#pragma once

#include "Category.h"
#include "ProximityCriterionTypeEnum.h"

#include <string>
#include <vector>

namespace produce_questions
{
    class Choice;

    class SimpleAnswerCategory : public Category
    {
    private:
        const int _questionCount;
        const std::string _questionListId;
        const int _choiceCount;
        const std::string _choiceListId;
        const double _distribParameterCorrection;
        const ProximityCriterionTypeEnum _proximityCriterionType;

        std::vector<const Choice *> _choiceVector;

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;

    public:
        SimpleAnswerCategory(unsigned int weightIndex, const MapParameters *mapParameters, int questionCount, const std::string& questionListId, int choiceCount,
                             const std::string& choiceListId, double distribParameterCorrection, ProximityCriterionTypeEnum proximityCriterionType);
    };
}
