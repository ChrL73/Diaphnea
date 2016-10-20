var mapServerInterface =
{
   createNewConnection: function(url, onConnected)
   {
      var connection = new Connection(url, function()
      {
         onConnected(connection);
      });

      function Connection(url, onConnected)
      {   
         var socket;
         var requestCounter = -1;
         var callBacks = {}; 

         $.getScript(url + '/socket.io/socket.io.js', function()
         {         
            socket = io(url);

            socket.on('mapIds', handleSimpleResponse);
            socket.on('mapLanguages', handleSimpleResponse);
            socket.on('mapName', handleSimpleResponse);
            socket.on('elementIds', handleSimpleResponse);

            onConnected();   

            function handleSimpleResponse(response)
            {
               var requestId = response.requestId.toString();
               callBacks[requestId](response.content);
               delete callBacks[requestId];
            }
         });  

         var mapIds;
         this.getMapIds = function(onMapIds)
         {
            if (mapIds)
            {
               onMapIds(mapIds);
            }
            else
            {
               var id = ++requestCounter;
               var request = { id: id };
               callBacks[id.toString()] = setMapIds;  
               socket.emit('getMapIds', request);
            }

            function setMapIds(responseContent)
            {
               mapIds = responseContent;
               onMapIds(responseContent);
            }
         };

         this.createNewMap = function(mapId, canvasId)
         {
            return new Map(mapId, canvasId);
         };

         function Map(mapId, canvasId)
         {
            var languages;
            this.getLanguages = function(onMapLanguages)
            {
               if (languages)
               {
                  onMapLanguages(languages);
               }
               else
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId };
                  callBacks[id.toString()] = setMapLanguages;
                  socket.emit('getMapLanguages', request);
               }

               function setMapLanguages(responseContent)
               {
                  languages = responseContent;
                  onMapLanguages(responseContent);
               }
            };

            names = {};
            this.getName = function(languageId, onMapName)
            {
               if (names[languageId])
               {
                  onMapName(names[languageId]);
               }
               else
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, languageId: languageId };
                  callBacks[id.toString()] = setMapName;
                  socket.emit('getMapName', request);
               }

               function setMapName(responseContent)
               {
                  names[languageId] = responseContent;
                  onMapName(responseContent);
               }
            };
            
            var elementsIds;
            this.getElementIds = function(onElementIds)
            {
               if (elementsIds)
               {
                  onElementIds(elementsIds);
               }
               else
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId };
                  callBacks[id.toString()] = setElementIds;
                  socket.emit('getElementIds', request);
               }

               function setElementIds(responseContent)
               {
                  elementsIds = responseContent;
                  onElementIds(responseContent);
               }
            };
            
            function Element(elementId)
            {
               
            }
         }
      }
   }
}

