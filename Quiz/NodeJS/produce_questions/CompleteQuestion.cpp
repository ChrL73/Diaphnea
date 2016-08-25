#include "CompleteQuestion.h"
#include "TextAndComment.h"
#include "RandomNumberGenerator.h"

namespace produce_questions
{
    CompleteQuestion::CompleteQuestion(const std::string& question, QuestionMultiplicityEnum multiplicity, int choiceCount) :
                _question(question), _multiplicity(multiplicity)
    {
        int i;
        for (i = 0; i < choiceCount; ++i) _choiceVector.push_back(0);
    }

    CompleteQuestion::~CompleteQuestion()
    {
        int i, n = _choiceVector.size();
        for (i = 0; i < n; ++i) delete _choiceVector[i];
    }

    void CompleteQuestion::addChoice(const std::string& text, const std::string& comment, bool rightAnswer)
    {
        int i = RandomNumberGenerator::getRandomInt(_choiceVector.size(), _filledLocationSet);
        _filledLocationSet.insert(i);
        _choiceVector[i] = new TextAndComment(text, comment);
        if (rightAnswer) _rightAnswerSet.insert(i);
    }

    const std::string& CompleteQuestion::getJson(void)
    {
        _json = "{\"question\":\"" + _question + "\",\"isMultiple\":"
            + (_multiplicity == produce_questions::MULTIPLE ? "true" : "false")
            + ",\"choices\":[";

        int i, n = _choiceVector.size();
        for (i = 0; i < n; ++i)
        {
            _json += "{\"text\":\"" + _choiceVector[i]->getText()
                     + "\",\"comment\":\"" + _choiceVector[i]->getComment() + "\",\"isRight\":"
                     + (_rightAnswerSet.find(i) != _rightAnswerSet.end() ? "true" : "false") + "}";
            if (i != n - 1) _json += ",";
        }

        _json += "]}";

        return _json;
    }
}
