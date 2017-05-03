#pragma once

#include <vector>
#include <string>

namespace produce_questions
{
    class CompleteQuestion;

    class RelationOrderQuestion
    {
    private:
        const std::string _question;
        const int _choiceCount;
        const std::string _choiceListId;

        std::vector<std::string> _choiceVector;

    public:
        RelationOrderQuestion(const std::string& question, int choiceCount, const std::string& choiceListId);

        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const;

    };
}
