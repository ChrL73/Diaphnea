$(function()
{            
   var url = 'http://albertine:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_France';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         var elementIds = map.getElementIds();
         map.loadElements(elementIds, function(elementArray)
         {
            $('#elementList').append('<p><input type="checkbox" id="all"/><label for="all">all</label></p>');
            
            elementArray.forEach(function(element)
            {
               var elementId = element.getId();
               $('#elementList').append('<p><input class="elt" type="checkbox" id="' + elementId + '"/><label for="' + elementId + '"> ' + element.getName('fr') + '</label></p>');
               $('#' + elementId).change(function()
               {
                  if ($('#' + elementId).prop('checked')) element.show();
                  else element.hide();
               });
            });
            
            $('#all').change(function()
            {
               var checked = Boolean($('#all').prop('checked'));
               
               $('.elt').each(function()
               {
                  $(this).prop('checked', checked).change();
               });
            });
         });
      }
   }
});
