#pragma once

#include <string>

namespace produce_questions
{
    class QuestionProducer
    {
    private:
        const std::string _levelId;
        const std::string _languageId;

    public:
        QuestionProducer(const std::string& levelId, const std::string& languageId) : _levelId(levelId), _languageId(languageId) {}

        int run(void);
    };
}
