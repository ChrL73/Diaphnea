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
            
            var visibleElements = {};
            var items = {};
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
               
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds, width: canvas.width, height: canvas.height };
               callBacks[id.toString()] = renderInit;
               socket.emit('render', request);
            }
               
            function renderInit(renderInfo)
            {
               console.log(renderInfo);      
               
               xFocus = renderInfo.xFocus;
               yFocus = renderInfo.yFocus;
               scale = renderInfo.scale;
               
               var resolution = renderInfo.resolution;
               var requestedLooks = {};
               renderInfo.items.forEach(function(itemInfo) { checkRenderData(itemInfo, true);} );

               function checkRenderData(itemInfo, firstCall)
               {
                  var itemId = itemInfo.id;
                  var lookId = itemInfo.lk;

                  if (firstCall)
                  {
                     if (!items[itemId]) items[itemId] = [];
                     if (!items[itemId][resolution])
                     {                           
                        var id = ++requestCounter;
                        var request = { id: id, mapId: mapId, itemId: itemId, resolution: resolution };
                        callBacks[id.toString()] = setItemData;
                        contexts[id.toString()] = { itemInfo: itemInfo, resolution: resolution }
                        socket.emit('getItemData', request)
                     }

                     if (!looks[lookId] && !requestedLooks[lookId])
                     {
                        requestedLooks[lookId] = true;
                        var id = ++requestCounter;
                        var request = { id: id, mapId: mapId, lookId: lookId };
                        callBacks[id.toString()] = setLookData;
                        contexts[id.toString()] = { itemInfo: itemInfo }
                        socket.emit('getLook', request)
                     }
                  }

                  if (items[itemId] && items[itemId][resolution] && looks[lookId]) renderItem(itemInfo, resolution);
               }

               function setItemData(itemData, context)
               {
                  console.log(itemData);
                  
                  var item = { type: itemData.type };
                  if (item.type == 'line')
                  {
                     var polyline = new fabric.Polyline(itemData.points, polylineOptions);
                     item.polyline = polyline;
                     item.top0 = polyline.top;
                     item.left0 = polyline.left;
                  }
                  
                  items[context.itemInfo.id][context.resolution] = item;
                  checkRenderData(context.itemInfo, false);
               }

               function setLookData(lookData, context)
               {
                  console.log(lookData);
                  looks[context.itemInfo.lk] = lookData; 
                  checkRenderData(context.itemInfo, false);
               }
            }

            function renderItem(itemInfo, resolution)
            {
               console.log('Render item ' + itemInfo.id + ' (resolution ' + resolution + ')...');
               
               var item = items[itemInfo.id][resolution];
               var look = looks[itemInfo.lk];
               
               if (item.type == 'line')
               {
                  var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter1 + scale);
                  var polyline = item.polyline;
                  polyline.stroke = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
                  polyline.strokeWidth = look.size * sizeFactor;
                  polyline.top = (item.top0 - 0.5 * polyline.strokeWidth - yFocus) * scale + 0.5 * canvas.height;   
                  polyline.left = (item.left0 - 0.5 * polyline.strokeWidth - xFocus) * scale + 0.5 * canvas.width;
                  polyline.scaleX = scale;
                  polyline.scaleY = scale;
                  if (!item.added)
                  {
                     canvas.add(polyline);
                     item.added = true;
                  }
               }
            }
         }
      }
   }
}
