$(function()
{            
   document.addEventListener('wheel', function(e) { e.preventDefault(); });
   $('#svg').hide();
   
   var url = 'http://albertine:3001';
   mapServerInterface.createNewConnection(url, onConnected, onError);
   
   function onError(error)
   {
      console.error(error);
   }
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_France';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         resizeCanvas();
         
         window.onresize = function()
         {
            resizeCanvas();
            map.redraw();
         }

         function resizeCanvas()
         {
            var w = window.innerWidth - $('#elementList').width() - $('#svgExport').width() - 100;
            if (w < 100) w = 100;
            $('#canvas').attr('width', w);
            $('#canvas').attr('height', window.innerHeight * 0.95);
            $('#svg').attr('width', w);
            $('#svg').attr('height', window.innerHeight * 0.95);
            $('#elementList').height((window.innerHeight * 0.95).toString() + 'px');
         }
         
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
            
            $('#potential').click(function()
            {
               map.requestPotentialImage();
            });
            
            $('#svgExport').click(function()
            {
               map.requestSvg();
            });
         });
      }
   }
});
