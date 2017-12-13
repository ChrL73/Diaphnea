#pragma once

#include <string>

namespace produce_questions
{
    class AttributeOrderChoice
    {
    private:
        const std::string _choiceText;
        const std::string _comment;
        const int _minIndex;
        const std::string _mapId;

    public:
        AttributeOrderChoice(const std::string& choiceText, const std::string& comment, int minIndex, const std::string& mapId) :
            _choiceText(choiceText), _comment(comment), _minIndex(minIndex), _mapId(mapId) {}

        const std::string& getChoiceText(void) const { return _choiceText; }
        const std::string& getComment(void) const { return _comment; }
        int getMinIndex(void) const { return _minIndex; }
        const std::string& getMapId(void) const { return _mapId; }
    };
}
