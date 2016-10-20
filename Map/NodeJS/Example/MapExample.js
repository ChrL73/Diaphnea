$(function()
{
   var url = 'http://192.168.50.138:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      mapServerConnection.getMapIds(function(mapIds)
      {
         console.log(mapIds);
      });
         
      var canvasId = 'canvas';
      var mapId = '_Gabon';
      var map = mapServerConnection.createNewMap(mapId, canvasId);
      
      map.getLanguages(function(mapLanguages)
      {
         console.log(mapLanguages);
      });
      
      map.getName('fr', function(mapName)
      {
         console.log(mapName);
      });
      
      map.getElementIds(function(elementIds)
      {
         console.log(elementIds);
      });
   }
});
