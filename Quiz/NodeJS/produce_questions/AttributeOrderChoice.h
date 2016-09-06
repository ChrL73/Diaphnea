#pragma once

#include <string>

namespace produce_questions
{
    class AttributeOrderChoice
    {
    private:
        const std::string _choiceText;
        const std::string _comment;
        const int _minIndex;

    public:
        AttributeOrderChoice(const std::string& choiceText, const std::string& comment, int minIndex) :
            _choiceText(choiceText), _comment(comment), _minIndex(minIndex) {}

        const std::string& getChoiceText(void) const { return _choiceText; }
        const std::string& getComment(void) const { return _comment; }
        int getMinIndex(void) const { return _minIndex; }
    };
}
