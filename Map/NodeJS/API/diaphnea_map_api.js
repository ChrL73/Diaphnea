var mapServerInterface =
{
   createNewConnection: function(url, onConnected)
   {
      var connection = new Connection(function()
      {
         onConnected(connection);
      });

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
            var visibleElements = {};
            var items = {};
            var looks = {};
            
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
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               callBacks[id.toString()] = renderStep;
               socket.emit('render', request);
               
               function renderStep(data)
               {
                  if (data.items)
                  {
                     console.log(data);
                     var resolution = data.resolution;

                     data.items.forEach(function(item)
                     {
                        var itemId = item.id;
                        var look = item.lk;
                        if (!items[itemId]) items[itemId] = [];
                        if (!items[itemId][resolution])
                        {                           
                           var id = ++requestCounter;
                           var request = { id: id, mapId: mapId, itemId: itemId, resolution: resolution };
                           callBacks[id.toString()] = setItemInfo;
                           contexts[id.toString()] = { itemId: itemId, resolution: resolution }
                           socket.emit('getItemInfo', request)
                        }
                     });
                  }
                  /*else if (data...)
                  {
                     
                  }*/
               }
               
               function setItemInfo(data, context)
               {
                  items[context.itemId][context.resolution] = data;
                  console.log(data);
               }
            }
         }
      }
   }
}

