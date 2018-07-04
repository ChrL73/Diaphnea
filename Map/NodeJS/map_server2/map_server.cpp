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

#include "MapServer.h"

#include <cstring>
#include <string>

int main(int argc, char *argv[])
{
    bool softExit = false;
    if (argc >= 2 && strcmp(argv[1], "softExit") == 0) softExit = true;

    int cleanThreadSleepMs = 5;
    int checkTimeoutSleepMs = 200;
    time_t timeoutInSeconds = 300;

    map_server::MapServer mapServer(timeoutInSeconds, cleanThreadSleepMs, checkTimeoutSleepMs, softExit);

    int result = mapServer.run();

    return result;
}
