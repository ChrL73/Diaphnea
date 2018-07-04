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

#include "QuestionProducer.h"

//#include <ctime>
//#include <iostream>

int main(void)
{
    //clock_t t0 = std::clock();

    produce_questions::QuestionProducer questionProducer;
    int result = questionProducer.run();

    //std::cout << std::endl << 0.001 * static_cast<double>(std::clock() - t0)  << "ms" << std::endl;

    return result;
}
