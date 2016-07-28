#pragma once

#include <string>

namespace produce_questions
{
    class SimpleAnswerQuestion
    {
    private:
        const std::string _question;
        const std::string _answer;
        const std::string _comment;

    public:
        SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment) :
            _question(question), _answer(answer), _comment(comment) {}

        const std::string& getQuestion(void) const { return _question; }
        const std::string& getAnswer(void) const { return _answer; }
        const std::string& getComment(void) const { return _comment; }

    };
}

