/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#pragma once

#include <set>

namespace produce_questions
{
    class RandomNumberGenerator
    {
    private:
        static bool _initialized;
        static void initialize(void);
        static unsigned int _shift1[32];
        static unsigned int _shift2[32];
        static int _counter1;
        static int _counter2;

        static unsigned int shift1(void);
        static unsigned int shift2(void);
        static unsigned int getRandomInt(void);
        static double getRandomDouble (void);
        static double inverseCumulativeDistribution (double distribParameter, double y);

    public:
        static int verify(void);
        static unsigned int getRandomInt(int valueCount);
        static unsigned int getRandomInt(int valueCount, const std::set<unsigned int>& excludedValues);
        static unsigned int getRandomInt (int valueCount, double distribParameter);
        static unsigned int getRandomInt (int valueCount, double distribParameter, std::set<unsigned int>& excludedValues);
    };
}

