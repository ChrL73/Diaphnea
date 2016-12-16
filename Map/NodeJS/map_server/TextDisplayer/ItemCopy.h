#pragma once

#include <vector>

namespace map_server
{
    class RepulsiveCenter;

    class ItemCopy
    {
    private:
        std::vector<const RepulsiveCenter *> _repulsiveCenterVector;

    public:
        virtual ~ItemCopy() {}

        void addRepulsiveCenter(RepulsiveCenter *repulsiveCenter) { _repulsiveCenterVector.push_back(repulsiveCenter); }

    };
}

