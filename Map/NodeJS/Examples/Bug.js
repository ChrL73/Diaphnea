$(function()
{
   var url = 'http://albertine:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      var canvasId = 'canvas';
      var mapId = '_France';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
   }
      
   function onMapLoaded(map)
   {
      var elementIds = map.getElementIds();
      map.loadElements(elementIds, onElementLoaded);
      
      function onElementLoaded(elements)
      {
         map.pushState('s1');
         
         elements.forEach(function(element)
         {
            element.show();
         });
         
         // Different behaviours for t=100 and t=200 (but bugs in the 2 cases)
         var t = 100;
         //var t = 200;
         
         setTimeout(function()
         {
            map.pushState('s2');
            map.popState('s1');
            setTimeout(function()
            {
               map.popState('s2');
            },
            2000);
         },
         t);
      }
   }
   
});

// The bug has been fixed thanks to the 'popCounter' variable in 'diaphnea_map_api.js'
