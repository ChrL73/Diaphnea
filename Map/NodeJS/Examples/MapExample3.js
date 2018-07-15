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
   $('#connectionForm').submit(function(e)
   {
      e.preventDefault();
      var url = 'http://' + $('#serverInput').val() + ':' + $('#portInput').val();
      mapServerInterface.createNewConnection(url, onConnected, onError);
   });

   function onError(error)
   {
      console.error(error);
      $('#errorSpan').text('Error: ' + error.message);
      $('#errorModal').modal('show');
   }
   
   $('#save').click(function(e) { e.preventDefault(); });
   $('#restore').click(function(e) { e.preventDefault(); });
   $('#svgExport').click(function(e) { e.preventDefault(); });
   $('#colorApply').click(function(e) { e.preventDefault(); });
   $('#colorRestore').click(function(e) { e.preventDefault(); });
   
   $('#closeCustomColor').click(resetColorSelect);
   
   function resetColorSelect()
   {
      $('#customColor').hide();
      $('.colorInputs').show();
      $('#colorSelect').val('__');
   }
   
   var timeoutId;

   function onConnected(mapServerConnection)
   {
      $('#mapForm').css('display', 'block');
      $('#connectionForm').css('display', 'none');
      
      $('#versionDiv').text('Server version: ' + mapServerConnection.getServerVersion());

      var mapIds = mapServerConnection.getMapIds();
      mapIds.forEach(function(mapId)
      {
         var mapName = mapId;
         while(mapName.charAt(0) === '_') mapName = mapName.substr(1);

         $('#mapSelect').append('<option value="' + mapId + '">' + mapName + '</option>');
      });
      
      var mapCounter = -1;
      var mapInfos = {};

      onMapChange();
      $('#mapSelect').change(onMapChange);
      
      function onMapChange()
      {
         $('#categoryDiv').empty();
         loadMap($('#mapSelect').val());
      }
      
      function loadMap(mapId)
      {
         $('.mapCanvas').hide();
         $('#linkGroup').hide();
         resetColorSelect();
         
         if (mapInfos[mapId])
         {
            $('#' + mapInfos[mapId].canvasId).show();
            onMapLoaded(mapInfos[mapId].map, true);
         }
         else
         {
            ++mapCounter;
            var canvasId = 'canvas' + mapCounter;
            mapInfos[mapId] = { canvasId: canvasId, elementsById: {}, selection: {}, fillingStyle: '0_', linkDepth: 1, linkThreshold: 50, restoreDisabled: true };
            
            $('#column1').append('<canvas class="mapCanvas" id="' + canvasId + '"></canvas>');
            $('#' + canvasId).attr('width', $('#column1').width());
            $('#' + canvasId).attr('height', window.innerHeight - 36);
            
            mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
         }

         function onMapLoaded(map, reload)
         {
            $('#home__').off();
            
            $('#svgExport').off();
            $('#svgExport').click(function(e)
            {
               e.preventDefault();
               map.requestSvg();
            });
            
            $('#save').off();
            $('#save').click(function(e)
            {
               e.preventDefault();
               map.pushState('toto');
               mapInfos[mapId].savedSelection = JSON.parse(JSON.stringify(mapInfos[mapId].selection));
               mapInfos[mapId].savedLanguage = mapInfos[mapId].language;
               mapInfos[mapId].savedFillingStyle = mapInfos[mapId].fillingStyle;
               mapInfos[mapId].restoreDisabled = false;
               $('#restore').prop('disabled', false);
            });
            
            $('#restore').off();
            $('#restore').prop('disabled', mapInfos[mapId].restoreDisabled);
            $('#restore').click(function(e)
            {
               e.preventDefault();
               map.popState('toto');
               mapInfos[mapId].selection = JSON.parse(JSON.stringify(mapInfos[mapId].savedSelection)); 
               updateCategories(mapInfos[mapId].elements, map, mapInfos[mapId]);
               mapInfos[mapId].language = mapInfos[mapId].savedLanguage;
               $('#languageSelect').val(mapInfos[mapId].language);
               mapInfos[mapId].fillingStyle = mapInfos[mapId].savedFillingStyle;
               $('#fillingStyleSelect').val(mapInfos[mapId].fillingStyle);
               mapInfos[mapId].restoreDisabled = true;
               $('#restore').prop('disabled', true);
            });
            
            if (!reload) { mapInfos[mapId].map = map; }
            
            $('#depthInput').val(mapInfos[mapId].linkDepth);
            $('#thresholdInput').val(mapInfos[mapId].linkThreshold);
            
            $('#depthInput').off();
            $('#depthInput').change(function()
            {
               var value = $('#depthInput').val();
               value = parseInt(value, 10);
               if (!isNaN(value) && value >=0 && value <= 20) { mapInfos[mapId].linkDepth = value; }
               $('#depthInput').val(mapInfos[mapId].linkDepth);
            });
            
            $('#thresholdInput').off();
            $('#thresholdInput').change(function()
            {
               var value = $('#thresholdInput').val();
               value = parseInt(value, 10);
               if (!isNaN(value) && value >=1 && value <= 1000) { mapInfos[mapId].linkThreshold = value; }
               $('#thresholdInput').val(mapInfos[mapId].linkThreshold);
            });
            
            $('#languageSelect').off();
            $('#fillingStyleSelect').off();
            $('#colorSelect').off();
            $('#languageSelect').empty();
            var languages = map.getLanguages();
            languages.forEach(function(language)
            {
               $('#languageSelect').append('<option value="' + language.id + '">' + language.name + '</option>');          
            });
            
            $('#fillingStyleSelect').val(mapInfos[mapId].fillingStyle);
            
            if (reload)
            {
               $('#languageSelect').val(mapInfos[mapId].language);
            }
            else
            {
               mapInfos[mapId].language = $('#languageSelect').val();
               map.setLanguage($('#languageSelect').val());
               mapInfos[mapId].fillingStyle = $('#fillingStyleSelect').val();
               map.setFillingStyle($('#fillingStyleSelect').val());
            }
                   
            $('#languageSelect').change(function()
            {
               resetColorSelect();
               var languageId = $('#languageSelect').val();
               map.setLanguage(languageId);
               mapInfos[mapId].language = languageId;
               map.redraw();
               $('#linkGroup').hide();
               updateCategories(mapInfos[mapId].elements, map, mapInfos[mapId]);
            });
            
            clearTimeout(timeoutId);
            timeoutId = setTimeout(f, 1000);
            function f()
            {
               updateColors(map, mapInfos[mapId].language);
               timeoutId = setTimeout(f, 1000);
            }
            
            $('#colorSelect').on('focus', function()
            {
               updateColors(map, mapInfos[mapId].language);
            });
            
            $('#colorSelect').change(function()
            {
               var colorId = $('#colorSelect').val();
               if (colorId == '__') return;
               
               $('#colorName').html($('#colorSelect option:selected').text());
               $('.colorInputs').hide();
               $('#customColor').show();
               
               var color = map.getColorInfo(colorId);
               
               $('#r1').text(color.defaultR);
               $('#r2').val(color.customR);             
               $('#g1').text(color.defaultG);
               $('#g2').val(color.customG);          
               $('#b1').text(color.defaultB);
               $('#b2').val(color.customB);          
               $('#a1').text(color.defaultA);
               $('#a2').val(color.customA);
               
               var r2 = color.customR;
               $('#r2').change(function()
               {
                  var value = $('#r2').val();
                  value = parseInt(value, 10);
                  if (!isNaN(value) && value >=0 && value <= 255) { r2 = value; }
                  $('#r2').val(r2);
                  showColors();
               });
               
               var g2 = color.customG;
               $('#g2').change(function()
               {
                  var value = $('#g2').val();
                  value = parseInt(value, 10);
                  if (!isNaN(value) && value >=0 && value <= 255) { g2 = value; }
                  $('#g2').val(g2);
                  showColors();
               });
               
               var b2 = color.customB;
               $('#b2').change(function()
               {
                  var value = $('#b2').val();
                  value = parseInt(value, 10);
                  if (!isNaN(value) && value >=0 && value <= 255) { b2 = value; }
                  $('#b2').val(b2);
                  showColors();
               });
               
               var a2 = color.customA;
               $('#a2').change(function()
               {
                  var value = $('#a2').val();
                  value = parseInt(value, 10);
                  if (!isNaN(value) && value >=0 && value <= 255) { a2 = value; }
                  $('#a2').val(a2);
                  showColors();
               });
               
               showColors();
               
               function showColors()
               {
                  $("#c1").css('background-color', 'rgba(' + color.defaultR + ', ' + color.defaultG + ', ' + color.defaultB + ', ' + (color.defaultA / 255.0) + ')');
                  $("#c2").css('background-color', 'rgba(' + r2 + ', ' + g2 + ', ' + b2 + ', ' + (a2 / 255.0) + ')');
               }
               
               $('#colorApply').off();
               $('#colorApply').click(function(e)
               {
                  e.preventDefault();
                  map.setCustomColor(colorId, r2, g2, b2, a2);
                  map.redraw();
               });
               
               $('#colorRestore').off();
               $('#colorRestore').click(function(e)
               {
                  e.preventDefault();
                  map.restoreDefaultColor(colorId);
                  map.redraw();
               });
            });
               
            $('#fillingStyleSelect').change(function()
            {
               var fillingStyle = $('#fillingStyleSelect').val();
               map.setFillingStyle(fillingStyle);
               mapInfos[mapId].fillingStyle = fillingStyle;
               map.redraw();
            });
         
            window.onresize = function()
            {
               resizeCanvas();
               map.redraw();
            };
            
            if (reload)
            {
               updateCategories(mapInfos[mapId].elements, map, mapInfos[mapId]);
            }
            else
            {
               var elementIds = map.getElementIds();
               map.loadElements(elementIds, function(elementArray)
               {
                  var i, n = elementArray.length;
                  for (i = 0; i < n; ++i)
                  {
                     mapInfos[mapId].elementsById[elementIds[i]] = elementArray[i];
                  }
                  
                  mapInfos[mapId].elements = elementArray;
                  updateCategories(elementArray, map, mapInfos[mapId]);
               });
            }
            
            $('#home__').click(function()
            {
               $('#linkGroup').hide();
               updateCategories(mapInfos[mapId].elements, map, mapInfos[mapId]);
            });
         }
      }
   }
   
   function updateColors(map, languageId)
   {
      var allColorIds = map.getColorIds();
      var colorIds = [];
      allColorIds.forEach(function(colorId)
      {
         if (!$('#colorCheckbox').prop('checked') || map.isColorVisible(colorId))
         {
            colorIds.push(colorId);
         }
      });   
      
      var options = $('#colorSelect > option');
      var changed = (options.length != colorIds.length + 1);
      
      if (!changed)
      {
         options.each(function(i, e)
         {
            if (i != 0 && $(e).val() != colorIds[i - 1])
            {
               changed = true;
               return false;
            }
         });
      }
      
      if(changed)
      {
         $('#colorSelect').empty();
         $('#colorSelect').append('<option value="__"></option>');

         colorIds.forEach(function(colorId)
         {
            var color = map.getColorInfo(colorId, languageId);
            $('#colorSelect').append('<option value="' + colorId + '">' + color.name + '</option>');
         });
      }
   }
   
   function updateCategories(elementArray, map, mapInfo)
   {
      $('#categoryDiv').empty();

      var categories = map.getCategories();
      var categoryArray = [];
      categories.forEach(function(category)
      {
         categoryArray.push([]);
      });

      elementArray.forEach(function(element)
      {
         categoryArray[element.getCategoryIndex()].push(element);
      });

      var html = '<div class="panel panel-default" style="margin-bottom:0px;">'
         + '<div class="panel-heading">'
         + '<input id="all__" type="checkbox"><span> All (' + elementArray.length + ')</span>'
         + '<span class="glyphicon glyphicon-triangle-top pull-right" id="collpaseAllIcon"'
         + 'data-toggle="collapse" data-target="#collapseAll" aria-controls="collapseAll"></span>'
         + '</div></div>'
         + '<div id="collapseAll" class="collpase"></div>';
      $('#categoryDiv').append(html);

      $('#collapseAll').on('shown.bs.collapse', function()
      {
         $('#collpaseAllIcon').removeClass('glyphicon-triangle-bottom').addClass('glyphicon-triangle-top');
      });
      $('#collapseAll').on('hidden.bs.collapse', function()
      {
         $('#collpaseAllIcon').removeClass('glyphicon-triangle-top').addClass('glyphicon-triangle-bottom');
      });

      var atLeastOneIndeterminate = false;
      var checkedCategoryCount = 0;
      var languageId = $('#languageSelect').val();
      categories.forEach(function(category, i)
      {
         if (categoryArray[i].length > 0)
         {
            var categoryName = category[languageId];
            categoryArray[i].sort(function(a, b) { return a.getName(languageId).localeCompare(b.getName(languageId)); });
            var status = addCategory(categoryName, i, categoryArray[i], languageId, map, mapInfo);

            if (status == 1) atLeastOneIndeterminate = true;
            else if (status == 2) ++checkedCategoryCount;

            var a = '#category_' + i;
            $(a).change(function()
            {
               var checked = Boolean($(a).prop('checked'));
               $('#collapse_' + i).find('input').prop('checked', checked).change();
            });
         }
      });

      $('#collapseAll').collapse();

      $('#all__').change(function()
      {
         var checked = Boolean($('#all__').prop('checked'));
         $('#collapseAll').find('.categoryInput').prop('checked', checked).change();
      });     

      if (checkedCategoryCount == categories.length)
      {
         $('#all__').prop('checked', true);
      }
      else if (atLeastOneIndeterminate || checkedCategoryCount > 0)
      {
         $('#all__').prop('indeterminate', true);
      }
   }
   
   function addCategory(categoryName, categoryIndex, elementArray, languageId, map, mapInfo)
   {
      var html = '<div class="panel panel-default" style="margin-bottom:0px;">'
         + '<div class="panel-heading">'
         + '<input type="checkbox" id="category_' + categoryIndex + '" class="categoryInput"><span> ' + categoryName + ' (' + elementArray.length + ')</span>'
         + '<span class="glyphicon glyphicon-triangle-bottom pull-right" id="collpaseIcon_' + categoryIndex + '"'
         + 'data-toggle="collapse" data-target="#collapse_' + categoryIndex + '" aria-controls="collapse_' + categoryIndex + '"></span>'
         + '</div>'
         + '<div class="collapse" id="collapse_' + categoryIndex + '">'
         + '<div class="panel-body">';
      
      var selection = mapInfo.selection;
      var checkedElementCount = 0;
      elementArray.forEach(function(element)
      {
         var checked = selection[element.getId()];
         html += '<p><input' + (checked ? ' checked' : '') + ' type="checkbox" id="' +
                 element.getId() + '" style="margin-left:12px;"><span> ' + element.getName(languageId) +
                 '</span><span id="arrow_' + element.getId() + '"class="glyphicon glyphicon-arrow-right pull-right" style="cursor:pointer;"></span></p>';    
         if (checked) ++checkedElementCount;
      });
      
      html += '</div></div></div>'; 
      
      $('#collapseAll').append(html);
      
      var returnValue = 0;
      if (checkedElementCount == elementArray.length)
      {
         $('#category_' + categoryIndex).prop('checked', true);
         returnValue = 2;
      }
      else if (checkedElementCount != 0)
      {
         $('#category_' + categoryIndex).prop('indeterminate', true);
         returnValue = 1;
      }
      
      $('#collapse_' + categoryIndex).on('shown.bs.collapse', function(e)
      {
         $('#collpaseIcon_' + categoryIndex).removeClass('glyphicon-triangle-bottom').addClass('glyphicon-triangle-top');
         e.stopImmediatePropagation();
      });
      $('#collapse_' + categoryIndex).on('hidden.bs.collapse', function(e)
      {
         $('#collpaseIcon_' + categoryIndex).removeClass('glyphicon-triangle-top').addClass('glyphicon-triangle-bottom');
         e.stopImmediatePropagation();
      });
       
      elementArray.forEach(function(element)
      {
         $('#' + element.getId()).change(function()
         {
            if ($('#' + element.getId()).prop('checked'))
            {
               element.show();
               selection[element.getId()] = true;
            }
            else
            {
               element.hide();
               selection[element.getId()] = false;
            }
            
            var checkCount = 0;
            $('#collapse_' + categoryIndex).find('input').each(function()
            {
               if ($(this).prop('checked')) ++checkCount;
            });
            
            if (checkCount == 0)
            {
               $('#category_' + categoryIndex).prop('checked', false);
               $('#category_' + categoryIndex).prop('indeterminate', false);
            }
            else if (checkCount == elementArray.length)
            {
               $('#category_' + categoryIndex).prop('checked', true);
               $('#category_' + categoryIndex).prop('indeterminate', false);
            }
            else
            {
               $('#category_' + categoryIndex).prop('indeterminate', true);
            }
            
            checkCount = 0;
            var indeterminateCount = 0;
            var categoryCount = 0;
            $('#collapseAll').find('.categoryInput').each(function()
            {
               ++categoryCount;
               if ($(this).prop('checked')) ++checkCount;
               if ($(this).prop('indeterminate')) ++indeterminateCount;
            });
            
            if (indeterminateCount > 0)
            {
               $('#all__').prop('indeterminate', true);
            }
            else
            {
               if (checkCount == 0)
               {
                  $('#all__').prop('checked', false);
                  $('#all__').prop('indeterminate', false);
               }
               else if (checkCount == categoryCount)
               {
                  $('#all__').prop('checked', true);
                  $('#all__').prop('indeterminate', false);
               }
               else
               {
                  $('#all__').prop('indeterminate', true);
               }
            }
         });
         
         $('#arrow_' + element.getId()).click(function()
         {
            $('#refresh__').off();
            $('#refresh__').click(f);
            f();
            
            function f()
            {
               var elements = getLinkedElements(element, mapInfo);
               updateCategories(elements, map, mapInfo);
               
               $('#currentElement').html(element.getName($('#languageSelect').val()));
               $('#linkGroup').show();
            } 
         });
      });
      
      return returnValue;
   }
      
   function getLinkedElements(element, mapInfo)
   {
      var linkedElements = {};
      linkedElements[element.getId()] = true;
      
      var i = 0;
      while (i < mapInfo.linkDepth)
      {
         var elementsToAdd = [];
            
         Object.getOwnPropertyNames(linkedElements).forEach(function(elementId)
         { 
            var linkedElements1 = mapInfo.elementsById[elementId].getLinkedElements1();
            if (i < 2 || linkedElements1.length < mapInfo.linkThreshold) { Array.prototype.push.apply(elementsToAdd, linkedElements1); }
         });        
         ++i; 
         
         if (i < mapInfo.linkDepth)
         {
            Object.getOwnPropertyNames(linkedElements).forEach(function(elementId)
            { 
               var linkedElements2 = mapInfo.elementsById[elementId].getLinkedElements2();
               if (i < 2 || linkedElements2.length < mapInfo.linkThreshold) { Array.prototype.push.apply(elementsToAdd, linkedElements2); }
            });
            ++i;
         }
         
         elementsToAdd.forEach(function(elementId)
         {
            linkedElements[elementId] = true;
         });
      }
      
      var elements = [];
      Object.getOwnPropertyNames(linkedElements).forEach(function(elementId)
      { 
         elements.push(mapInfo.elementsById[elementId]);
      });
      
      return elements;
   }
   
   resizeCanvas();

   function resizeCanvas()
   {
      $('#column1').height((window.innerHeight - 36).toString() + 'px');
      $('#column2').height((window.innerHeight - 8).toString() + 'px');      
      
      $('.mapCanvas').attr('width', $('#column1').width());
      $('.mapCanvas').attr('height', window.innerHeight - 36);
   }
   
});
