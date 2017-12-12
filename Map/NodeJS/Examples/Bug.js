$(function()
{
   var url = 'http://192.168.50.73:3001';
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
         //var t = 100;
         //var t = 200;
         
         // 'popCounter' correction corrects the bug for t >~ 110
         // but not for 100 <= t <~110
         var t = 105;
         
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
