#pragma once

#include <string>

namespace map_server
{
    class DatabaseError
    {
    private:
        const std::string _file;
        const std::string _function;
        const int _line;

    public:
        DatabaseError(const std::string& file, const std::string& function, int line) :
            _file(file), _function(function), _line(line) {}

        const std::string& getFile(void) const { return _file; }
        const std::string& getFunction(void) const { return _function; }
        int getLine(void) const { return _line; }
    };
}
