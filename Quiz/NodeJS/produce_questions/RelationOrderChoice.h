#pragma once

#include <string>

namespace produce_questions
{
    class RelationOrderChoice
    {
    private:
        const std::string _text;
        const std::string _mapId;

    public:
        RelationOrderChoice(const std::string& text, const std::string& mapId) : _text(text), _mapId(mapId) {}

        const std::string& getText(void) const { return _text; }
        const std::string& getMapId(void) const { return _mapId; }
    };
}
