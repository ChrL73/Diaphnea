#pragma once

#include <vector>

namespace produce_questions
{
    class Category;

    class Level
    {
    private:
        const int _questionCount;
        const int _choiceCount;
        const int _categoryCount;
        const int _weightSum;
        const double _distribParameter;
        const std::vector<Category *> _categoryVector;

    public:
        Level(int questionCount, int choiceCount, int categoryCount, int weightSum, double distribParameter, const std::vector<Category *> categoryVector) :
            _questionCount(questionCount), _choiceCount(choiceCount), _categoryCount(categoryCount), _weightSum(weightSum), _distribParameter(distribParameter), _categoryVector(categoryVector) {}

        int getQuestionCount(void) const { return _questionCount; }
        int getChoiceCount(void) const { return _choiceCount; }
        int getCategoryCount(void) const { return _categoryCount; }
        int getWeightSum(void) const { return _weightSum; }
        double getDistribParameter(void) const { return _distribParameter; }
        Category *getCategories(int i) const { return _categoryVector[i]; }
    };
}
