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
   document.addEventListener('wheel', function(e) { e.preventDefault(); });
   $('#svg').hide();
   
   var url = 'http://albertine:3001';
   mapServerInterface.createNewConnection(url, onConnected, onError);
   
   function onError(error)
   {
      console.error(error);
   }
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_France';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         resizeCanvas();
         
         window.onresize = function()
         {
            resizeCanvas();
            map.redraw();
         }

         function resizeCanvas()
         {
            var w = window.innerWidth - $('#elementList').width() - $('#svgExport').width() - 100;
            if (w < 100) w = 100;
            $('#canvas').attr('width', w);
            $('#canvas').attr('height', window.innerHeight * 0.95);
            $('#svg').attr('width', w);
            $('#svg').attr('height', window.innerHeight * 0.95);
            $('#elementList').height((window.innerHeight * 0.95).toString() + 'px');
         }
         
         var elementIds = map.getElementIds();
         map.loadElements(elementIds, function(elementArray)
         {
            $('#elementList').append('<p><input type="checkbox" id="all"/><label for="all">all</label></p>');
            
            elementArray.forEach(function(element)
            {
               var elementId = element.getId();
               $('#elementList').append('<p><input class="elt" type="checkbox" id="' + elementId + '"/><label for="' + elementId + '"> '
                                        + element.getName('fr') /*+ ' (' + map.getCategories()[element.getCategoryIndex()]['fr'] + ')'*/ + '</label></p>');
               $('#' + elementId).change(function()
               {
                  if ($('#' + elementId).prop('checked')) element.show();
                  else element.hide();
               });
            });
            
            $('#all').change(function()
            {
               var checked = Boolean($('#all').prop('checked'));
               
               $('.elt').each(function()
               {
                  $(this).prop('checked', checked).change();
               });
            });
            
            $('#potential').click(function()
            {
               map.requestPotentialImage();
            });
            
            $('#svgExport').click(function()
            {
               map.requestSvg();
            });
         });
      }
   }
});
