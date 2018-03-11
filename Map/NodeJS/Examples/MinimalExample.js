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
      var elementId = 'Bretagne';
      map.loadElement(elementId, onElementLoaded);
   }
   
   function onElementLoaded(element)
   {
      element.show();
   }
});
