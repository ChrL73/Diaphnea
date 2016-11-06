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
         var socket;
         var requestCounter = -1;
         var callBacks = {};
         var contexts = {};
         
         var mapIds;
         this.getMapIds = function() { return mapIds; }

         $.getScript(url + '/socket.io/socket.io.js', function()
         {         
            socket = io(url);
            
            socket.on('mapIds', function(response)
            {
               if (!mapIds)
               {
                  mapIds = response.content;
                  onConnected();
               }
            });
            
            var id = ++requestCounter;
            var request = { id: id };
            socket.emit('getMapIds', request);
            
            socket.on('res', function(response)
            {
               var requestId = response.requestId.toString();
               callBacks[requestId](response.content, contexts[requestId]);
               delete callBacks[requestId];
               delete contexts[requestId];
               
               //console.log(callBacks);
            });
         });  
         
         this.loadMap = function(mapId, canvasId, onMapLoaded)
         {
            var id = ++requestCounter;
            var request = { id: id, mapId: mapId };
            callBacks[id.toString()] = setMapInfo;
            socket.emit('getMapInfo', request);

            function setMapInfo(mapInfo)
            {
               onMapLoaded(new Map(mapId, canvasId, mapInfo));
            }
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
               callBacks[id.toString()] = setElementInfo;
               socket.emit('getElementInfo', request);

               function setElementInfo(elementInfo)
               {
                  onElementLoaded(new Element(elementId, elementInfo));
               }
            }
            
            this.loadElements = function(elementIds, onElementsLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               callBacks[id.toString()] = setElementsInfo;
               socket.emit('getElementsInfo', request);

               function setElementsInfo(elementsInfo)
               {
                  var elements = [];
                  elementsInfo.forEach(function(elementInfo, i)
                  {
                     elements.push(new Element(elementIds[i], elementInfo));
                  });
                  onElementsLoaded(elements);
               }
            }
               
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
                  callBacks[id.toString()] = renderInit;
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
               
            function renderInit(renderInfo)
            {
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
                     callBacks[id.toString()] = setItemData;
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
                  callBacks[id.toString()] = setLookData;
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
            }

            function setItemData(itemData, context)
            {
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
            }

            function setLookData(lookData, context)
            {
               //console.log(lookData);
               
               looks[context.lookId] = lookData;     
               context.itemIdArray.forEach(function(itemId)
               {
                  renderItem(itemId, context.lookId);
               });
            }
            
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
