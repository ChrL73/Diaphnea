#pragma once

#include <string>

namespace produce_questions
{
    class TextAndComment
    {
    private:
        const std::string _text;
        const std::string _comment;

    public:
        TextAndComment(const std::string& text, const std::string& comment) :
            _text(text), _comment(comment) {}

        const std::string& getText(void) const { return _text; }
        const std::string& getComment(void) const { return _comment; }

    };
}
