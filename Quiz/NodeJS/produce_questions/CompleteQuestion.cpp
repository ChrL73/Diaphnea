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
}
