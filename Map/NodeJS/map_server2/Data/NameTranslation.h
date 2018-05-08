#pragma once

namespace map_server
{
    class NameTranslation
    {
    private:
        // Array of ElementNames
        int _nameCount;
        int _names;

    public:
        int getNameCount(void) const { return _nameCount; }
        int getLineCount(int nameIndex) const;
        const char *getLine(int nameIndex, int lineIndex) const;
    };
}
