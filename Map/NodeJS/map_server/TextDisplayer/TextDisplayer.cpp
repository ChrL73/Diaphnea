#include "TextDisplayer.h"
#include "ItemCopy.h"

namespace map_server
{
	TextDisplayer::~TextDisplayer()
	{
		int i, n = _itemVector.size();
		for (i = 0; i < n; ++i) delete _itemVector[i];
	}

    void TextDisplayer::start(void)
    {

    }
}
