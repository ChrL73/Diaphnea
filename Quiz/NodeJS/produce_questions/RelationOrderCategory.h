#pragma once

#include "Category.h"

#include <string>

namespace produce_questions
{
    class RelationOrderCategory : public Category
    {
    private:
        const int _questionCount;
        const std::string _questionListId;
        const double _distribParameterCorrection;

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;

    public:
        RelationOrderCategory(unsigned int weightIndex, const MapParameters *mapParameters, int questionCount, const std::string& questionListId, double distribParameterCorrection):
            Category(weightIndex, mapParameters), _questionCount(questionCount), _questionListId(questionListId), _distribParameterCorrection(distribParameterCorrection) {}

    };
}
