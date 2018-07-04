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
      var canvasId = 'canvas';
      var mapId = '_France';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
   }
      
   function onMapLoaded(map)
   {
      var elementIds = map.getElementIds();
      map.loadElements(elementIds, onElementLoaded);
      
      function onElementLoaded(elements)
      {
         map.pushState('s1');
         
         elements.forEach(function(element)
         {
            element.show();
         });
         
         // Different behaviours for t=100 and t=200 (but bugs in the 2 cases)
         var t = 100;
         //var t = 200;
         
         setTimeout(function()
         {
            map.pushState('s2');
            map.popState('s1');
            setTimeout(function()
            {
               map.popState('s2');
            },
            2000);
         },
         t);
      }
   }
   
});

// The bug has been fixed thanks to the 'popCounter' variable in 'map_api.js'
