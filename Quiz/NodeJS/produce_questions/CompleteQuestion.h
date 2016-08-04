#pragma once

#include "QuestionMultiplicityEnum.h"

#include <string>
#include <vector>
#include <set>

namespace produce_questions
{
    class TextAndComment;

    class CompleteQuestion
    {
    private:
        const std::string _question;
        const QuestionMultiplicityEnum _multiplicity;
        std::vector<TextAndComment *> _choiceVector;
        std::set<int> _rightAnswerSet;
        std::set<unsigned int> _filledLocationSet;
        std::string _json;

    public:
        CompleteQuestion(const std::string& question, QuestionMultiplicityEnum multiplicity, int choiceCount);
        ~CompleteQuestion();

        void addChoice(const std::string& text, const std::string& comment, bool rightAnswer);
        const std::string& getJson(void);

    };
}
