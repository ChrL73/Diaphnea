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
         lockMovementY: true,
         selectable: false
      };
      
      var polygonOptions =
      {
         strokeWidth: 0,
         hasControls: false,
         hasBorders: false,
         lockMovementX: true,
         lockMovementY: true,
         selectable: false
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
            socket.emit('mapIdsReq', request);
            
            socket.on('mapIdsRes', function(response)
            {
               if (!mapIds)
               {
                  mapIds = response.content;
                  onConnected();
               }
            });
         
            socket.on('mapInfoRes', function(response)
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
            socket.emit('mapInfoReq', request);
         }
         
         function Map(mapId, canvasId, mapInfo)
         {
            var thisMap = this;
            var canvas = new fabric.Canvas(canvasId);
            canvas.hoverCursor = 'default';
            canvas.selection = false;
            var renderCanvasTimeout;
            
            var visibleElements = {};
            var items = {};
            var addedItems = {};
            var itemsToRemove = {};
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
               socket.emit('elementInfoReq', request);
            }
            
            socket.on('elementInfoRes', function(response)
            {
               var context = getContext(response);
               context.onElementLoaded(new Element(context.elementId, response.content));
            });
            
            this.loadElements = function(elementIds, onElementsLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               contexts[id.toString()] = { elementIds: elementIds, onElementsLoaded: onElementsLoaded };
               socket.emit('elementsInfoReq', request);
            }
            
            socket.on('elementsInfoRes', function(response)
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
            
            var renderTimeout;
            this.render = function()
            {
               clearTimeout(renderTimeout);
               renderTimeout = setTimeout(startRender, 100);
            }
            
            var lastRenderRequestId;
            function startRender()
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
                  var context = {}
                  if (scale && xFocus && yFocus)
                  {
                     request.scale = scale;
                     request.xFocus = xFocus;
                     request.yFocus = yFocus;
                     context.ignoreServerScale = true;
                  }
                  contexts[id.toString()] = context;
                  lastRenderRequestId = id;
                  socket.emit('renderReq', request);
               }
               else
               {
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
                  {
                     removeItem(items[itemId]);
                  });
               }
            }
               
            socket.on('renderRes', function(response)
            {
               var context = getContext(response);
               if (response.requestId != lastRenderRequestId) return;
               
               var renderInfo = response.content;
               //console.log(renderInfo);      
               
               if (!context.ignoreServerScale)
               {
                  xFocus = renderInfo.xFocus;
                  yFocus = renderInfo.yFocus;
                  scale = renderInfo.scale;
               }
               
               var lookToItems = {};
               var itemsToRender_fullId = {};
               var itemsToRender_idWithoutRes = {};
               var itemsToRender_idAndLook = [];
               
               renderInfo.items.forEach(function(itemInfo)
               {
                  var itemId = itemInfo[0].toString();
                  var lookId = itemInfo[1];
                  var resolution = itemInfo[2];
                  if (resolution || resolution === 0) itemId += '_' + resolution;
                  
                  itemsToRender_fullId[itemId] = true;
                  itemsToRender_idWithoutRes[itemInfo[0]] = true;
         
                  if (!items[itemId])
                  {                           
                     var id = ++requestCounter;
                     var request = { id: id, mapId: mapId, itemId: itemInfo[0], resolution: (resolution ? resolution : 0) };
                     contexts[id.toString()] = { itemId: itemId, lookId: lookId };
                     socket.emit('itemDataReq', request);
                  }

                  if (!looks[lookId])
                  {
                     if (!lookToItems[lookId]) lookToItems[lookId] = [];
                     lookToItems[lookId].push(itemId);
                  } 
                  
                  if (!addedItems[itemId] || items[itemId].lookId != lookId)
                  {
                     itemsToRender_idAndLook.push({ itemId: itemId, lookId: lookId });
                  }
               });
               
               Object.getOwnPropertyNames(lookToItems).forEach(function(lookId)
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, lookId: lookId };
                  contexts[id.toString()] = { lookId: lookId, itemIdArray: lookToItems[lookId] };
                  socket.emit('lookReq', request);
               });
               
               Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
               {
                  if (!itemsToRender_fullId[itemId])
                  {
                     var idWithoutRes = itemId.substring(0, itemId.indexOf('_'));
                     if (itemsToRender_idWithoutRes[idWithoutRes])
                     {
                        itemsToRemove[itemId] = true;
                     }
                     else
                     {      
                        var item = items[itemId];
                        removeItem(item);
                     }
                  }
               });
               
               itemsToRender_idAndLook.forEach(function(ids)
               {
                  renderItem(ids.itemId, ids.lookId);
               });
            });

            socket.on('itemDataRes', function (response)
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
               else if (item.type == 'polygon')
               {
                  var polygon = new fabric.Polygon(itemData.points, polygonOptions);
                  item.polygon = polygon;
                  item.top0 = polygon.top;
                  item.left0 = polygon.left;
               }
               else if (item.type == 'point')
               {
                  var circle = new fabric.Circle(
                  {
                     radius: 0.5,
                     left: itemData.x - 0.5,
                     top: itemData.y - 0.5,
                     stroke: 'black',
                     hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true, selectable: false
                  });
                  item.circle = circle;
                  item.top0 = circle.top;
                  item.left0 = circle.left;
               }

               items[item.id] = item;
               renderItem(context.itemId, context.lookId);
            });

            socket.on('lookRes', function (response)
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
               if (item) item.lookId = lookId;
               
               var look = looks[lookId];
                  
               if (item && look)
               {
                  // console.log('Render item ' + itemId);
                  
                  Object.getOwnPropertyNames(itemsToRemove).forEach(function(itemId2)
                  {
                     if (itemId != itemId2 && itemId.substring(0, itemId.indexOf('_')) == itemId2.substring(0, itemId2.indexOf('_')))
                     {
                        removeItem(items[itemId2]);
                     }
                  });
                  
                  var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter2 + scale);

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
                        clearTimeout(renderCanvasTimeout);
                        renderCanvasTimeout = setTimeout(function() { canvas.renderAll(); }, 50);
                     }
                     item.zIndex = look.zI;
                     canvas.moveTo(polyline, getIndex(item));
                  }
                  else if (item.type == 'polygon')
                  {
                     var polygon = item.polygon;
                     polygon.fill = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
                     polygon.top = (item.top0 - yFocus) * scale + 0.5 * canvas.height;   
                     polygon.left = (item.left0 - xFocus) * scale + 0.5 * canvas.width;
                     polygon.scaleX = scale;
                     polygon.scaleY = scale;
                     if (!addedItems[itemId])
                     {
                        canvas.add(polygon);
                        addedItems[itemId] = true;
                     }
                     else
                     {
                        clearTimeout(renderCanvasTimeout);
                        renderCanvasTimeout = setTimeout(function() { canvas.renderAll(); }, 50);
                     }
                     item.zIndex = look.zI;
                     canvas.moveTo(polygon, getIndex(item));
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
                        clearTimeout(renderCanvasTimeout);
                        renderCanvasTimeout = setTimeout(function() { canvas.renderAll(); }, 50);
                     }
                     item.zIndex = look.zI;
                     canvas.moveTo(circle, getIndex(item));
                  }
               }
            }
            
            function getIndex(item)
            {
               var count = 0;
               Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
               {
                  if (itemId != item.Id && items[itemId].zIndex > item.zIndex) ++count;      
               });
               return count;
            }
            
            function removeItem(item)
            {
               var itemId = item.id;
               
               if (addedItems[itemId])
               {
                  if (item.type == 'line')
                  {
                     canvas.remove(item.polyline);
                  }
                  else if (item.type == 'polygon')
                  {
                     canvas.remove(item.polygon);
                  }
                  else if (item.type == 'point')
                  {    
                     canvas.remove(item.circle);
                  }
                  
                  delete addedItems[itemId];
                  delete itemsToRemove[itemId];
               }
            }
            
            var mustTranslate = false;
            var xRef, yRef;
            
            canvas.on('mouse:down', function(arg)
            {
               xRef = arg.e.clientX;
               yRef = arg.e.clientY;
               mustTranslate = true;
               //canvas.defaultCursor = 'move';   
            });
            
            canvas.on('mouse:move', function(arg)
            {
               if (mustTranslate && (arg.e.buttons & 1))
               {
                  var dx = arg.e.clientX - xRef;
                  var dy = arg.e.clientY - yRef;

                  xFocus -= dx / scale;
                  yFocus -= dy / scale;
                  
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
                  {
                     var item = items[itemId];
                     if (item.type == 'line')
                     {
                        var polyline = item.polyline;
                        polyline.top = (item.top0 - 0.5 * polyline.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                        polyline.left = (item.left0 - 0.5 * polyline.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                     }
                     else if (item.type == 'polygon')
                     {
                        var polygon = item.polygon;
                        polygon.top = (item.top0 - yFocus) * scale + 0.5 * canvas.height;   
                        polygon.left = (item.left0 - xFocus) * scale + 0.5 * canvas.width;
                     }
                     else if (item.type == 'point')
                     {
                        var circle = item.circle;
                        circle.top = (item.top0 - 0.5 * circle.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                        circle.left = (item.left0 - 0.5 * circle.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                     }
                  });

                  clearTimeout(renderCanvasTimeout);
                  renderCanvasTimeout = setTimeout(function() { canvas.renderAll(); }, 1);
                  thisMap.render();

                  xRef = arg.e.clientX;
                  yRef = arg.e.clientY;
               }
            });
            
            canvas.on('mouse:up', function(arg)
            {
               mustTranslate = false;
               //canvas.defaultCursor = 'default';
            });
            
            canvas.on('mouse:wheel', function(arg)
            {
               var delta = arg.e.deltaY;
               var geoSize = Math.sqrt(canvas.width * canvas.width + canvas.height * canvas.height) / scale;
               var geoSize0 = geoSize;

               if (delta > 0) geoSize *= 1.15;
               else geoSize /= 1.15;

               if (geoSize < mapInfo.zoomMinDistance) geoSize = mapInfo.zoomMinDistance;
               else if (geoSize > mapInfo.zoomMaxDistance) geoSize = mapInfo.zoomMaxDistance;

               if (geoSize != geoSize0)
               {   
                  var zoomFactor = geoSize0 / geoSize;
                  if (delta > 0)
                  {
                     xFocus += (0.5 * canvas.width - arg.e.clientX) * (1.0 - zoomFactor) / scale;
                     yFocus += (0.5 * canvas.height - arg.e.clientY) * (1.0 - zoomFactor) / scale;
                  }
                  else
                  {
                     xFocus += (0.5 * canvas.width - arg.e.clientX) * (1.0 / zoomFactor - 1.0) / scale;
                     yFocus += (0.5 * canvas.height - arg.e.clientY) * (1.0 / zoomFactor - 1.0) / scale;
                  }
                  
                  scale *= zoomFactor;
                  var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter2 + scale);

                  Object.getOwnPropertyNames(addedItems).forEach(function(itemId)
                  {
                     var item = items[itemId];
                     var look = looks[item.lookId];
                     if (item.type == 'line')
                     {
                        var polyline = item.polyline;
                        polyline.strokeWidth = look.size * sizeFactor;
                        polyline.top = (item.top0 - 0.5 * polyline.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                        polyline.left = (item.left0 - 0.5 * polyline.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                        polyline.scaleX = scale;
                        polyline.scaleY = scale;
                     }
                     else if (item.type == 'polygon')
                     {
                        var polygon = item.polygon;
                        polygon.top = (item.top0 - yFocus) * scale + 0.5 * canvas.height;   
                        polygon.left = (item.left0 - xFocus) * scale + 0.5 * canvas.width;
                        polygon.scaleX = scale;
                        polygon.scaleY = scale;
                     }
                     else if (item.type == 'point')
                     {
                        var circle = item.circle;
                        circle.radius = 0.5 * look.size * sizeFactor;
                        circle.strokeWidth = sizeFactor;
                        circle.top = (item.top0 - 0.5 * circle.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                        circle.left = (item.left0 - 0.5 * circle.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                        circle.scaleX = scale;
                        circle.scaleY = scale;
                     }
                  });
                   
                  clearTimeout(renderCanvasTimeout);
                  renderCanvasTimeout = setTimeout(function() { canvas.renderAll(); }, 1);
                  thisMap.render();
               }
            });
         }
      }
   }
}
