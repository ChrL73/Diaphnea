#pragma once

#include <string>

namespace produce_questions
{
    class MapSubParameters;

    class MapIdInfo
    {
    private:
        const std::string _id;
        const MapSubParameters * const _parameters;

    public:
        MapIdInfo(const std::string& id, const MapSubParameters *parameters) : _id(id), _parameters(parameters) {}

        const std::string& getId(void) const { return _id; }
        const MapSubParameters *getParameters(void) const { return _parameters; }
    };
}
