$(function()
{            
   var url = 'http://192.168.50.84:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_Gabon';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         var elementIds = map.getElementIds();
         map.loadElements(elementIds, function(elementArray)
         {
            elementArray.forEach(function(element)
            {
               var elementId = element.getId();
               $('#elementList').append('<p><input type="checkbox" id="' + elementId + '"/><label for="' + elementId + '"> ' + element.getName('fr') + '</label></p>');
               $('#' + elementId).change(function()
               {
                  if ($('#' + elementId).prop('checked')) element.show();
                  else element.hide();
               });
            });
            
            $('#render').click(function()
            {
               map.render();
            });
         });
      }
   }
});
