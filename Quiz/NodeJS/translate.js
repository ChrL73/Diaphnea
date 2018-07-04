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

var fr = require('./languages/fr');
var en = require('./languages/en');
//var fr2 = require('./languages/fr2');

module.exports = function(languageId, textId)
{
   if (!languageId) return { languages:
   [
      { id: 'fr', name: 'Français'},
      { id: 'en', name: 'English'},
      //{ id: 'fr2', name: 'Français 2'}
   ] };
   
   if (!textId)
   {
      if (languageId == 'fr') return { texts: fr };
      if (languageId == 'en') return { texts: en };
      //if (languageId == 'fr2') return { texts: fr2 };
   }
   
   if (languageId == 'fr') return fr[textId];
   if (languageId == 'en') return en[textId];
   //if (languageId == 'fr2') return fr2[textId];
};
