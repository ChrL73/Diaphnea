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

   function onConnected(mapServerConnection)
   {
      $('#mapForm').css('display', 'block');
      $('#connectionForm').css('display', 'none');

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
         loadMap($('#mapSelect').val());
      }
      
      function loadMap(mapId)
      {
         $('canvas').hide();
         
         if (mapInfos[mapId])
         {
            $('#' + mapInfos[mapId].canvasId).show();
            onMapLoaded(mapInfos[mapId].map, true);
         }
         else
         {
            ++mapCounter;
            var canvasId = 'canvas' + mapCounter;
            mapInfos[mapId] = { canvasId: canvasId, selection: {} };
            
            $('#column1').append('<canvas id="' + canvasId + '"></canvas>');
            $('#' + canvasId).attr('width', $('#column1').width());
            $('#' + canvasId).attr('height', window.innerHeight - 36);
            
            mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
         }

         function onMapLoaded(map, reload)
         {
            if (!reload) { mapInfos[mapId].map = map; }
            
            $('#languageSelect').off();
            $('#languageSelect').empty();
            var languages = map.getLanguages();
            languages.forEach(function(language)
            {
               $('#languageSelect').append('<option value="' + language.id + '">' + language.name + '</option>');          
            });
            
            if (reload)
            {
               $('#languageSelect').val(mapInfos[mapId].language).change();
            }
            else
            {
               mapInfos[mapId].language = $('#languageSelect').val();
               map.setLanguage($('#languageSelect').val());
            }
                   
            $('#languageSelect').change(function()
            {
               var languageId = $('#languageSelect').val();
               map.setLanguage(languageId);
               mapInfos[mapId].language = languageId;
               map.redraw();
               updateCategories(mapInfos[mapId].elements);
            });
         
            window.onresize = function()
            {
               resizeCanvas();
               map.redraw();
            }
            
            if (reload)
            {
               updateCategories(mapInfos[mapId].elements);
            }
            else
            {
               var elementIds = map.getElementIds();
               map.loadElements(elementIds, function(elementArray)
               {
                  mapInfos[mapId].elements = elementArray;
                  updateCategories(elementArray);
               });
            }
                             
            function updateCategories(elementArray)
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
                     var status = addCategory(categoryName, i, categoryArray[i], languageId, mapInfos[mapId].selection);
                     
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
         }
      }
   }
   
   function addCategory(categoryName, categoryIndex, elementArray, languageId, selection)
   {
      var html = '<div class="panel panel-default" style="margin-bottom:0px;">'
         + '<div class="panel-heading">'
         + '<input type="checkbox" id="category_' + categoryIndex + '" class="categoryInput"><span> ' + categoryName + ' (' + elementArray.length + ')</span>'
         + '<span class="glyphicon glyphicon-triangle-bottom pull-right" id="collpaseIcon_' + categoryIndex + '"'
         + 'data-toggle="collapse" data-target="#collapse_' + categoryIndex + '" aria-controls="collapse_' + categoryIndex + '"></span>'
         + '</div>'
         + '<div class="collapse" id="collapse_' + categoryIndex + '">'
         + '<div class="panel-body">';
      
      var checkedElementCount = 0;
      elementArray.forEach(function(element)
      {
         var checked = selection[element.getId()];
         html += '<p><input' + (checked ? ' checked' : '') + ' type="checkbox" id="' +
                 element.getId() + '" style="margin-left:12px;"><span> ' + element.getName(languageId)+ '</span></p>';    
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
      });
      
      return returnValue;
   }
   
   resizeCanvas();

   function resizeCanvas()
   {
      $('#column1').height((window.innerHeight - 36).toString() + 'px');
      $('#column2').height((window.innerHeight - 8).toString() + 'px');      
      
      $('canvas').attr('width', $('#column1').width());
      $('canvas').attr('height', window.innerHeight - 36);
   }
   
});
