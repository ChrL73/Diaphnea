var socket;
var requestCounter = -1;
var callBacks = {};

var diaphneaMap =
{   
   connect: function(url, onConnected)
   {
      $.getScript(url + '/socket.io/socket.io.js', function()
      {         
         socket = io(url);
               
         socket.on('mapIds', handleSimpleResponse);
         socket.on('mapName', handleSimpleResponse);
         
         onConnected();   
         
         function handleSimpleResponse(response)
         {
            var requestId = response.requestId.toString();
            callBacks[requestId](response.content);
            delete callBacks[requestId];
         }
      });  
   },
   
   getMapIds: function(onMapIds)
   {
      var id = ++requestCounter;
      var request = { id: id };
      callBacks[id.toString()] = onMapIds;
      
      socket.emit('getMapIds', request);
   },
   
   Map: function(mapId, canvasId)
   {
      this.getMapName = function(languageId, onMapName)
      {
         var id = ++requestCounter;
         var request = { id: id, mapId: mapId, languageId: languageId };
         callBacks[id.toString()] = onMapName;

         socket.emit('getMapName', request);
      }

      this.getElementName = function(elementId, languageId, onElementName)
      {

      };


   }
}

