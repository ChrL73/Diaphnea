$(function()
{            
   $('#connectionForm').submit(function(e)
   {
      e.preventDefault();
      var url = 'http://' + $('#serverInput').val() + ':' + $('#portInput').val();
      mapServerInterface.createNewConnection(url, onConnected, onError);
   });

   function onError(error)
   {
      console.error(error);
      $('#errorSpan').text('Error: ' + error.message);
      $('#errorModal').modal('show');
   }

   function onConnected(mapServerConnection)
   {
      $('#mapForm').css('display', 'block');
      $('#connectionForm').css('display', 'none');

      var mapIds = mapServerConnection.getMapIds();
      mapIds.forEach(function(mapId)
      {
         var mapName = mapId;
         while(mapName.charAt(0) === '_') mapName = mapName.substr(1);

         $('#mapSelect').append('<option value="' + mapId + '">' + mapName + '</option>');
      });

      onMapChange();
      $('#mapSelect').change(onMapChange);
      
      function onMapChange()
      {
         loadMap($('#mapSelect').val());
      }
      
      function loadMap(mapId)
      {
         var canvasId = 'canvas';
         mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);

         function onMapLoaded(map)
         {
            $('#languageSelect').empty();
            var languages = map.getLanguages();
            languages.forEach(function(language)
            {
               $('#languageSelect').append('<option value="' + language.id + '">' + language.name + '</option>');          
            });
            
            var elementIds = map.getElementIds();
            map.loadElements(elementIds, function(elementArray)
            {

            });
         }
      }
   }
   
});
