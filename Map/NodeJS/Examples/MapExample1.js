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

$(function()
{
   var url = 'http://albertine:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      console.log(mapServerConnection.getMapIds());
      console.log(mapServerConnection.getServerVersion());
      
      var canvasId = 'canvas';
      var mapId = '_Gabon';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         console.log(map.getLanguages());
         console.log(map.getName('fr'));
         console.log(map.getElementIds());
         
         var libreville, estuaire, woleuNtem;
         
         map.loadElement('Libreville', onElementLoaded);
         
         function onElementLoaded(element)
         {
            libreville = element;
            console.log(libreville.getName('fr'));
            
            map.loadElements(['Estuaire', 'WoleuNtem', 'KomoOcean'], onElementsLoaded);
         }
         
         function onElementsLoaded(elements)
         {
            estuaire = elements[0];
            woleuNtem = elements[1];
            komoOcean = elements[2];
             
            console.log(estuaire.getName('fr'));
            console.log(woleuNtem.getName('fr'));
            
            libreville.show();
            estuaire.show();
            woleuNtem.show();
            komoOcean.show();
            woleuNtem.hide();
         }
      }
   }
});
