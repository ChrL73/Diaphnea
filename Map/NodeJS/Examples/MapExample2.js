$(function()
{            
   var url = 'http://192.168.50.91:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_Gabon';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         var elements = {};
         
         var elementIds = map.getElementIds();
         map.loadElements(elementIds, function(elementArray)
         {
            elementArray.forEach(function(element)
            {
               var elementId = element.getId();
               elements[elementId] = element;
               $('#elementList').append('<p><input type="checkbox" id="' + elementId + '"/><label for="' + elementId + '"> ' + element.getName('fr') + '</label></p>');
            });
            
            
         });
      }
   }
});
