#pragma once

#include "QuestionMultiplicityEnum.h"

#include <string>
#include <vector>
#include <set>

namespace produce_questions
{
    class TextAndComment;
    class MapIdInfo;
    class MapParameters;

    class CompleteQuestion
    {
    private:
        const MapParameters * const _mapParameters;
        const std::string _question;
        const QuestionMultiplicityEnum _multiplicity;
        std::vector<TextAndComment *> _choiceVector;
        std::vector<MapIdInfo *> _mapIdVector;
        std::set<int> _rightAnswerSet;
        std::set<unsigned int> _filledLocationSet;
        std::string _json;

    public:
        CompleteQuestion(const MapParameters *mapParameters, const std::string& question, QuestionMultiplicityEnum multiplicity, int choiceCount);
        ~CompleteQuestion();

        void addChoice(const std::string& text, const std::string& comment, bool rightAnswer);
        void addMapId(const std::string& id, int depth);
        const std::string& getJson(void);

    };
}
