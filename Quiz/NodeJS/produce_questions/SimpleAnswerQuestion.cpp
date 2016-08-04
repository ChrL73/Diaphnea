#include "SimpleAnswerQuestion.h"
#include "Choice.h"

#include <map>

namespace produce_questions
{
    SimpleAnswerQuestion::SimpleAnswerQuestion(const std::string& question, const std::string& answer, const std::string& comment, ProximityCriterionTypeEnum proximityCriterionType,
                                               double doubleCriterionValue, const std::string& stringCriterionValue, const std::vector<const Choice *>& choiceVector) :
            _question(question), _answer(answer), _comment(comment), _proximityCriterionType(proximityCriterionType),
            _doubleCriterionValue(doubleCriterionValue), _stringCriterionValue(stringCriterionValue)
    {
        if (_proximityCriterionType == produce_questions::STRING)
        {
            std::multimap<std::string, const Choice *> choiceMultimap;

            int i, n = choiceVector.size();
            for (i = 0; i < n; ++i)
            {
                const Choice *choice = choiceVector[i];
                choiceMultimap.insert(std::pair<std::string, const Choice *>(choice->getStringCriterionValue(), choice));
            }

            std::multimap<std::string, const Choice *>::const_iterator it1 = choiceMultimap.find(_stringCriterionValue);
            std::multimap<std::string, const Choice *>::const_iterator it2 = it1;
            ++it2;

            while (true)
            {
                if (it1 != choiceMultimap.begin())
                {
                    --it1;
                    const Choice *choice = (*it1).second;
                    if (choice->getChoiceText() != _answer) _sortedWrongChoiceVector1.push_back (choice);
                }
                if (it2 != choiceMultimap.end())
                {
                    const Choice *choice = (*it2).second;
                    if (choice->getChoiceText() != _answer) _sortedWrongChoiceVector2.push_back (choice);
                    ++it2;
                }
                if (it1 == choiceMultimap.begin() && it2 == choiceMultimap.end()) break;
            }
        }
        else if (_proximityCriterionType == produce_questions::NUMBER)
        {
            std::multimap<double, const Choice *> choiceMultimap;

            int i, n = choiceVector.size();
            for (i = 0; i < n; ++i)
            {
                const Choice *choice = choiceVector[i];
                if (choice->getChoiceText() != _answer)
                choiceMultimap.insert(std::pair<double, const Choice *>(choice->getDoubleCriterionValue(), choice));
            }

            std::multimap<double, const Choice *>::const_iterator it1 = choiceMultimap.find(_doubleCriterionValue);
            std::multimap<double, const Choice *>::const_iterator it2 = it1;
            ++it2;

            while (true)
            {
                if (it1 != choiceMultimap.begin())
                {
                    --it1;
                    const Choice *choice = (*it1).second;
                    if (choice->getChoiceText() != _answer) _sortedWrongChoiceVector1.push_back (choice);
                }
                if (it2 != choiceMultimap.end())
                {
                    const Choice *choice = (*it2).second;
                    if (choice->getChoiceText() != _answer) _sortedWrongChoiceVector2.push_back (choice);
                    ++it2;
                }
                if (it1 == choiceMultimap.begin() && it2 == choiceMultimap.end()) break;
            }
        }
        else
        {

        }
    }
}
