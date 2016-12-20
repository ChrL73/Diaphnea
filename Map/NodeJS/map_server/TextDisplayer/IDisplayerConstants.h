#pragma once

namespace map_server
{
    class Potential;

    class IDisplayerConstants
    {
    public:
        virtual ~IDisplayerConstants() {}

        virtual double getMaxRepulsionRatio(void) const = 0;
        virtual int getPotentialTableSize(void) const = 0;
        virtual const Potential *getMaxPotential(void) const = 0;
        virtual const Potential *getMinPotential(void) const = 0;
    };
}
