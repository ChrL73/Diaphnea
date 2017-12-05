#pragma once

namespace produce_questions
{
    class MapSubParameters;

    class MapParameters
    {
    private:
        const int _framingLevel;
        const MapSubParameters * const _questionParameters;
        const MapSubParameters * const _answerParameters;
        const MapSubParameters * const _wrongChoiceParameters;
        const bool _allAnswersSelectionMode;

    public:
        MapParameters(int framingLevel, const MapSubParameters *questionParameters, const MapSubParameters *answerParameters,
                      const MapSubParameters *wrongChoiceParameters, bool allAnswersSelectionMode) :
            _framingLevel(framingLevel), _questionParameters(questionParameters), _answerParameters(answerParameters),
            _wrongChoiceParameters(wrongChoiceParameters), _allAnswersSelectionMode(allAnswersSelectionMode) {}
        ~MapParameters();

        int getFramingLevel(void) const { return _framingLevel; }
        const MapSubParameters *getQuestionParameters(void) const { return _questionParameters; }
        const MapSubParameters *getAnswerParameters(void) const { return _answerParameters; }
        const MapSubParameters *getWrongChoiceParameters(void) const { return _wrongChoiceParameters; }
        bool getAllAnswersSelectionMode(void) const { return _allAnswersSelectionMode; }
    };
}
