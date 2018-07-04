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

#include <string>

namespace produce_questions
{
    class MapSubParameters;

    class MapIdInfo
    {
    private:
        const std::string _id;
        const MapSubParameters * const _parameters;

    public:
        MapIdInfo(const std::string& id, const MapSubParameters *parameters) : _id(id), _parameters(parameters) {}

        const std::string& getId(void) const { return _id; }
        const MapSubParameters *getParameters(void) const { return _parameters; }
    };
}
