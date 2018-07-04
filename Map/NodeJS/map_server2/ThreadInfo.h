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

#include <thread>
#include <future>
#include <string>

namespace map_server
{
    class ThreadInfo
    {
    private:
        std::thread _t;
        std::future<void> _f;
        std::string _str;

    public:
        ThreadInfo(std::thread&& t, std::future<void>&& f, const std::string&& str) :
            _t(std::move(t)), _f(std::move(f)), _str(std::move(str)) {}

        std::thread& getT(void) { return _t; }
        const std::future<void>& getF(void) const { return _f; }
        const std::string& getStr(void) const { return _str; }
    };
}
