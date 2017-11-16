#pragma once

#include <string>

namespace produce_questions
{
    class MapIdInfo
    {
    private:
        const std::string _id;
        const int _depth;

    public:
        MapIdInfo(const std::string& id, int depth) : _id(id), _depth(depth) {}

        const std::string& getId(void) const { return _id; }
        int getDepth(void) const { return _depth; }
    };
}
