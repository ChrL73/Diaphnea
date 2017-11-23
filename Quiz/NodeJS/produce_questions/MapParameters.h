#pragma once

#include <vector>
#include <string>

namespace produce_questions
{
    class MapParameters
    {
    private:
        const int _framingLevel;
        const int _questionDrawDepth;
        const int _answerDrawDepth;
        const int _wrongChoiceDrawDepth;
        const std::string _categorySelectionMode;
        const std::vector<std::string> _categories;

    public:
        MapParameters(int framingLevel, int questionDrawDepth, int answerDrawDepth, int wrongChoiceDrawDepth,
                      const std::string& categorySelectionMode, const std::vector<std::string>& categories) :
            _framingLevel(framingLevel), _questionDrawDepth(questionDrawDepth), _answerDrawDepth(answerDrawDepth), _wrongChoiceDrawDepth(wrongChoiceDrawDepth),
            _categorySelectionMode(categorySelectionMode), _categories(categories) {}

        int getFramingLevel(void) const { return _framingLevel; }
        int getQuestionDrawDepth(void) const { return _questionDrawDepth; }
        int getAnswerDrawDepth(void) const { return _answerDrawDepth; }
        int getWrongChoiceDrawDepth(void) const { return _wrongChoiceDrawDepth; }
        const std::string& getCategorySelectionMode(void) const { return _categorySelectionMode; }
        int getCategoryCount(void) const { return _categories.size(); }
        const std::string& getCategory(int i) const { return _categories[i]; }
    };
}
