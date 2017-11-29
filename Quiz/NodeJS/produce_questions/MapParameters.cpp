#include "MapParameters.h"
#include "MapSubParameters.h"

namespace produce_questions
{
    MapParameters::~MapParameters()
    {
        delete _questionParameters;
        delete _answerParameters;
        delete _wrongChoiceParameters;
    }
}
