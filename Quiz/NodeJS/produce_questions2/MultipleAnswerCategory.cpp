#include "MultipleAnswerCategory.h"
#include "RandomNumberGenerator.h"
#include "CompleteQuestion.h"
#include "MultipleAnswerQuestion.h"
#include "MapParameters.h"
#include "MapSubParameters.h"
#include "Choice.h"

#include <map>
#include <cmath>
#include <cstring>

namespace produce_questions
{
    CompleteQuestion *MultipleAnswerCategory::getNewQuestion(int choiceCount, double distribParameter) const
    {
        int draw = RandomNumberGenerator::getRandomInt(_questionCount);
        const MultipleAnswerQuestion *question = getQuestion(draw);

        CompleteQuestion *completeQuestion = new CompleteQuestion(getMapParameters(), question->getQuestion(), produce_questions::MULTIPLE, choiceCount);

        const MapSubParameters *questionParameters = getMapParameters()->getQuestionParameters();
        const MapSubParameters *answerParameters = getMapParameters()->getAnswerParameters();
        const MapSubParameters *wrongChoiceParameters = getMapParameters()->getWrongChoiceParameters();
        if (questionParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getQuestionMapId(), questionParameters);

        int valueCount = choiceCount - 1;
        if (valueCount > static_cast<int>(question->getAnswerCount())) valueCount = question->getAnswerCount();
        int rightChoiceCount = 1 + RandomNumberGenerator::getRandomInt(valueCount);

        std::vector<const Choice *> sortedWrongChoiceVector;

        std::set<std::string> answerSet;
        int i, n = question->getAnswerCount();
        for (i = 0; i < n; ++i) answerSet.insert(question->getAnswer(i));

        if (_proximityCriterionType == produce_questions::POINT_3D)
        {
            double x0 = question->getPointCriterionValueX();
            double y0 = question->getPointCriterionValueY();
            double z0 = question->getPointCriterionValueZ();

            std::multimap<double, const Choice *> choiceMultimap;

            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                if (answerSet.find(choice->getChoiceText()) == answerSet.end())
                {
                    double x = choice->getPointCriterionValue()[0];
                    double y = choice->getPointCriterionValue()[1];
                    double z = choice->getPointCriterionValue()[2];

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
                if (answerSet.find(choice->getChoiceText()) == answerSet.end() && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                {
                    sortedWrongChoiceVector.push_back(choice);
                }
            }
        }
        else
        {
            for (i = 0; i < _choiceCount; ++i)
            {
                const Choice *choice = getChoice(i);
                if (answerSet.find(choice->getChoiceText()) == answerSet.end() && strcmp(choice->getChoiceText(), question->getExcludedChoice()) != 0)
                {
                    sortedWrongChoiceVector.push_back(choice);
                }
            }
        }

        int wrongChoiceCount = sortedWrongChoiceVector.size();
        if (rightChoiceCount + wrongChoiceCount < choiceCount) rightChoiceCount = choiceCount - wrongChoiceCount;

        if (getMapParameters()->getAllAnswersSelectionMode() && answerParameters->getDrawDepth() != 0)
        {
            n = question->getAnswerCount();
            for (i = 0; i < n; ++i) completeQuestion->addMapId(question->getAnswerMapId(i), answerParameters);
        }

        std::set<unsigned int> excludedValues;
        for (i = 0; i < rightChoiceCount; ++i)
        {
            draw = RandomNumberGenerator::getRandomInt(question->getAnswerCount(), excludedValues);
            excludedValues.insert(draw);
            completeQuestion->addChoice(question->getAnswer(draw), question->getComment(draw), true);
            if (!getMapParameters()->getAllAnswersSelectionMode() && answerParameters->getDrawDepth() != 0) completeQuestion->addMapId(question->getAnswerMapId(draw), answerParameters);
        }

        excludedValues.clear();
        n = choiceCount - rightChoiceCount;
        for (i = 0; i < n; ++i)
        {
            if (_proximityCriterionType != produce_questions::POINT_3D) draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, excludedValues);
            else draw = RandomNumberGenerator::getRandomInt(wrongChoiceCount, distribParameter + _distribParameterCorrection, excludedValues);
            excludedValues.insert(draw);
            const Choice *wrongChoice = sortedWrongChoiceVector[draw];
            completeQuestion->addChoice(wrongChoice->getChoiceText(), wrongChoice->getComment(), false);
            if (wrongChoiceParameters->getDrawDepth() != 0) completeQuestion->addMapId(wrongChoice->getMapId(), wrongChoiceParameters);
        }

        return completeQuestion;
    }
}
