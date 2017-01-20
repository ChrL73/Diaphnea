#pragma once

#include <vector>
#include <string>

namespace map_server
{
    class ElementName
    {
    private:
        const std::vector<std::string> _lineVector;

    public:
        ElementName(const std::vector<std::string>& lineVector) : _lineVector(lineVector) {}

        int getLineCount(void) const { return _lineVector.size(); }
        const std::string& getLine(int i) const { return _lineVector[i]; }
    };
}
