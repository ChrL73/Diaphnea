var mapServerInterface =
{
   createNewConnection: function(url, onConnected)
   {
      var connection = new Connection(function()
      {
         onConnected(connection);
      });
      
      var polylineOptions =
      {
         strokeLineCap: 'round',
         strokeLineJoin: 'round',
         fill: '',
         hasControls: false,
         hasBorders: false,
         lockMovementX: true,
         lockMovementY: true
      };

      function Connection(onConnected)
      {   
         var mapIds;
         this.getMapIds = function() { return mapIds; }
         
         var socket;
         var requestCounter = -1;
            
         var contexts = {};
         function getContext(response)
         {
            var requestId = response.requestId.toString();
            var context = contexts[requestId];
            delete contexts[requestId];
            return context;
         }

         $.getScript(url + '/socket.io/socket.io.js', function()
         {         
            socket = io(url);
            
            var id = ++requestCounter;
            var request = { id: id };
            socket.emit('getMapIds', request);
            
            socket.on('mapIds', function(response)
            {
               if (!mapIds)
               {
                  mapIds = response.content;
                  onConnected();
               }
            });
         
            socket.on('mapInfo', function(response)
            {
               var context = getContext(response);
               context.onMapLoaded(new Map(context.mapId, context.canvasId, response.content));
            });
         });  
         
         this.loadMap = function(mapId, canvasId, onMapLoaded)
         {
            var id = ++requestCounter;
            var request = { id: id, mapId: mapId };
            contexts[id.toString()] = { mapId: mapId, canvasId: canvasId, onMapLoaded: onMapLoaded };
            socket.emit('getMapInfo', request);
         }
         
         function Map(mapId, canvasId, mapInfo)
         {
            var canvas = new fabric.Canvas(canvasId);
            canvas.hoverCursor = 'default';
            canvas.selection = false;
            var renderTimeout;
            
            var visibleElements = {};
            var items = {};
            var addedItems = {};
            var looks = {};
            var xFocus, yFocus, scale;
            
            this.getLanguages = function() { return mapInfo.languages; };
            this.getName = function(languageId) { return mapInfo.names[languageId]; };
            this.getElementIds = function() { return mapInfo.elementIds; };  
            
            this.loadElement = function(elementId, onElementLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementId: elementId };
               contexts[id.toString()] = { elementId: elementId, onElementLoaded: onElementLoaded };
               socket.emit('getElementInfo', request);
            }
            
            socket.on('elementInfo', function(response)
            {
               var context = getContext(response);
               context.onElementLoaded(new Element(context.elementId, response.content));
            });
            
            this.loadElements = function(elementIds, onElementsLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               contexts[id.toString()] = { elementIds: elementIds, onElementsLoaded: onElementsLoaded };
               socket.emit('getElementsInfo', request);
            }
            
            socket.on('elementsInfo', function(response)
            {
               var context = getContext(response);
               var elements = [];
               response.content.forEach(function(elementInfo, i)
               {
                  elements.push(new Element(context.elementIds[i], elementInfo));
               });
               context.onElementsLoaded(elements);
            });
               
            function Element(elementId, elementInfo)
            {
               this.getId = function() { return elementId; }
               this.getName = function(languageId) { return elementInfo.names[languageId]; }
               
               this.show = function() { visibleElements[elementId] = true; }
               this.hide = function() { visibleElements[elementId] = false; }
            }
            
            this.render = function()
            {
               var elementIds = [];
               Object.getOwnPropertyNames(visibleElements).forEach(function(elementId)
               {
                  if (visibleElements[elementId]) elementIds.push(elementId);
               });
               
               if (elementIds.length)
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, elementIds: elementIds, width: canvas.width, height: canvas.height };
                  contexts[id.toString()] = {};
                  socket.emit('render', request);
               }
               else
               {
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
                  {
                     removeItem(items[itemId]);
                  });
               }
            }
               
            socket.on('items', function(response)
            {
               var context = getContext(response);
               var renderInfo = response.content;
               //console.log(renderInfo);      
               
               xFocus = renderInfo.xFocus;
               yFocus = renderInfo.yFocus;
               scale = renderInfo.scale;
               
               var lookToItems = {};
               var itemsToRender = {};
               
               renderInfo.items.forEach(function(itemInfo)
               {
                  var itemId = itemInfo[0].toString();
                  var lookId = itemInfo[1];
                  var resolution = itemInfo[2];
                  if (resolution) itemId += '_' + resolution;
                  
                  itemsToRender[itemId] = true;
         
                  if (!items[itemId])
                  {                           
                     var id = ++requestCounter;
                     var request = { id: id, mapId: mapId, itemId: itemInfo[0], resolution: (resolution ? resolution : 0) };
                     contexts[id.toString()] = { itemId: itemId, lookId: lookId };
                     socket.emit('getItemData', request);
                  }

                  if (!looks[lookId])
                  {
                     if (!lookToItems[lookId]) lookToItems[lookId] = [];
                     lookToItems[lookId].push(itemId);
                  }

                  renderItem(itemId, lookId);
               });
               
               Object.getOwnPropertyNames(lookToItems).forEach(function(lookId)
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, lookId: lookId };
                  contexts[id.toString()] = { lookId: lookId, itemIdArray: lookToItems[lookId] };
                  socket.emit('getLook', request);
               });
               
               Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
               {
                  if (!itemsToRender[itemId])
                  {
                     var item = items[itemId];
                     removeItem(item);
                  }
               });
            });

            socket.on('itemData', function (response)
            {
               var context = getContext(response);
               var itemData = response.content;
               //console.log(itemData);

               var item = { id: context.itemId, type: itemData.type };
               if (item.type == 'line')
               {
                  var polyline = new fabric.Polyline(itemData.points, polylineOptions);
                  item.polyline = polyline;
                  item.top0 = polyline.top;
                  item.left0 = polyline.left;
               }
               else if (item.type == 'point')
               {
                  var circle = new fabric.Circle(
                  {
                     radius: 0.5,
                     left: itemData.x - 0.5,
                     top: itemData.y - 0.5,
                     stroke: 'black',
                     hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true
                  });
                  item.circle = circle;
                  item.top0 = circle.top;
                  item.left0 = circle.left;
               }

               items[item.id] = item;
               renderItem(context.itemId, context.lookId);
            });

            socket.on('look', function (response)
            {
               var context = getContext(response);
               var lookData = response.content;
               //console.log(lookData);
               
               looks[context.lookId] = lookData;     
               context.itemIdArray.forEach(function(itemId)
               {
                  renderItem(itemId, context.lookId);
               });
            });
            
            function renderItem(itemId, lookId)
            {
               var item = items[itemId];
               var look = looks[lookId];
                  
               if (item && look)
               {
                  //console.log('Render item ' + itemId;
                  
                  var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter1 + scale);

                  if (item.type == 'line')
                  {
                     var polyline = item.polyline;
                     polyline.stroke = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
                     polyline.strokeWidth = look.size * sizeFactor;
                     polyline.top = (item.top0 - 0.5 * polyline.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                     polyline.left = (item.left0 - 0.5 * polyline.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                     polyline.scaleX = scale;
                     polyline.scaleY = scale;
                     if (!addedItems[itemId])
                     {
                        canvas.add(polyline);
                        addedItems[itemId] = true;
                     }
                     else
                     {
                        clearTimeout(renderTimeout);
                        renderTimeout = setTimeout(function() { canvas.renderAll(); }, 50);
                     }
                     canvas.moveTo(polyline, -look.zI);
                  }
                  else if (item.type == 'point')
                  {
                     var circle = item.circle;
                     circle.fill = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
                     circle.radius = 0.5 * look.size * sizeFactor;
                     circle.strokeWidth = sizeFactor;
                     circle.top = (item.top0 - 0.5 * circle.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                     circle.left = (item.left0 - 0.5 * circle.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                     circle.scaleX = scale;
                     circle.scaleY = scale;
                     if (!addedItems[itemId])
                     {
                        canvas.add(circle);
                        addedItems[itemId] = true;
                     }
                     else
                     {
                        clearTimeout(renderTimeout);
                        renderTimeout = setTimeout(function() { canvas.renderAll(); }, 50);
                     }
                     canvas.moveTo(circle, -look.zI);
                  }
               }
            }
            
            function removeItem(item)
            {
               var itemId = item.id;
               
               if (addedItems[itemId])
               {
                  if (item.type == 'line')
                  {
                     addedItems[itemId] = false;
                     canvas.remove(item.polyline);
                  }
                  else if (item.type == 'point')
                  {
                     addedItems[itemId] = false;
                     canvas.remove(item.circle);
                  }
               }
            }
         }
      }
   }
}
