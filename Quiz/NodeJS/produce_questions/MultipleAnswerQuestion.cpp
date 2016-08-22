#include "MultipleAnswerQuestion.h"
#include "Point.h"
#include "Choice.h"
#include "TextAndComment.h"

#include <map>

namespace produce_questions
{
    MultipleAnswerQuestion::MultipleAnswerQuestion(const std::string& question, const std::vector<const TextAndComment *>& answerVector, const std::string& excludedChoice, ProximityCriterionTypeEnum proximityCriterionType,
                                                   const Point *pointCriterionValue, const std::vector<const Choice *>& choiceVector) : 
        _question(question), _answerVector(answerVector), _excludedChoice(excludedChoice), _proximityCriterionType(proximityCriterionType), _pointCriterionValue(pointCriterionValue)
    {
        std::set<std::string> answerSet;
        int i, n = answerVector.size();
        for (i = 0; i < n; ++i) answerSet.insert(answerVector[i]->getText());

        if (_proximityCriterionType == produce_questions::POINT_3D)
        {
            double x0 = _pointCriterionValue->getX();
            double y0 = _pointCriterionValue->getY();
            double z0 = _pointCriterionValue->getZ();

            std::multimap<double, const Choice *> choiceMultimap;

            n = choiceVector.size();
            for (i = 0; i < n; ++i)
            {
                const Choice *choice = choiceVector[i];
                if (answerSet.find(choice->getChoiceText()) == answerSet.end())
                {
                    double x = choice->getPointCriterionValue().getX();
                    double y = choice->getPointCriterionValue().getY();
                    double z = choice->getPointCriterionValue().getZ();

                    double dx = x - x0;
                    double dy = y - y0;
                    double dz = z - z0;

                    double d = sqrt(dx*dx + dy*dy + dz*dz);
                    choiceMultimap.insert(std::pair<double, const Choice *>(d, choice));
                }
            }

            std::multimap<double, const Choice *>::iterator it = choiceMultimap.begin();
            for (; it != choiceMultimap.end(); ++it)
            {
                const Choice *choice = (*it).second;
                if (answerSet.find(choice->getChoiceText()) == answerSet.end() && choice->getChoiceText() != _excludedChoice) _sortedWrongChoiceVector.push_back(choice);
            }
        }
        else
        {
            int i, n = choiceVector.size();
            for (i = 0; i < n; ++i)
            {
                const Choice *choice = choiceVector[i];
                if (answerSet.find(choice->getChoiceText()) == answerSet.end() && choice->getChoiceText() != _excludedChoice) _sortedWrongChoiceVector.push_back(choice);
            }
        }
    }

    MultipleAnswerQuestion::~MultipleAnswerQuestion()
    {
        delete _pointCriterionValue;
        int i, n = _answerVector.size();
        for (i = 0; i < n; ++i) delete _answerVector[i];
    }
}
