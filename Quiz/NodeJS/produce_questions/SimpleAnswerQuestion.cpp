#include "SimpleAnswerQuestion.h"

namespace produce_questions
{
    SimpleAnswerQuestion::SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment, ProximityCriterionTypeEnum proximityCriterionType) :
            _question(question), _answer(answer), _comment(comment), _proximityCriterionType(proximityCriterionType)
    {
        if (_proximityCriterionType == produce_questions::STRING)
        {

        }
        else if (_proximityCriterionType == produce_questions::NUMBER)
        {

        }
        else
        {

        }
    }
}
