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

var config = {};

config.port = process.env.PORT || 3001;

//config.debugDelay = 1000;
//config.displayRequests = 1;
//config.displayReponses = 1;
//config.displayStat = 1;
//config.mode = 'record';
//config.mode = 'replay';

module.exports = config;
