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
            
            socket.on('mapInfo', handleResponse);
            
            function handleResponse(response)
            {
               var requestId = response.requestId.toString();
               callBacks[requestId](response.content);
               delete callBacks[requestId];
            }
         });  
         
         this.loadMap = function(mapId, canvasId, onMapLoaded)
         {
            var map = new Map(mapId, canvasId, function()
            {
               onMapLoaded(map);
            });
         }
         
         function Map(mapId, canvasId, onMapLoaded)
         {
            var mapInfo;
            this.getLanguages = function() { return mapInfo.languages; };
            this.getName = function(languageId) { return mapInfo.names[languageId]; };
            this.getElementIds = function() { return mapInfo.elementIds; };
            
            var id = ++requestCounter;
            var request = { id: id, mapId: mapId };
            callBacks[id.toString()] = setMapInfo;
            socket.emit('getMapInfo', request);

            function setMapInfo(responseContent)
            {
               mapInfo = responseContent;
               onMapLoaded();
            }
            
            this.loadElement = function(elementId, onElementLoad)
            {
               var element = new Element(elementId, function()
               {
                  onElementLoad(element);
               });
               
               function Element(elementId, onElementLoad)
               {
                  var names;
                  this.getName = function(languageId) { return names[languageId]; }
                  
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, elementId: elementId };
                  callBacks[id.toString()] = setElementInfo;
                  socket.emit('getElementInfo', request);

                  function setElementInfo(responseContent)
                  {
                     names = responseContent;
                     onElementLoad();
                  }
               }
            }
         }
      }
   }
}

