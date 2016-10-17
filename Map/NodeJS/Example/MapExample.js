$(function()
{
   var url = 'http://192.168.50.89:3001';
   
   diaphneaMap.connect(url, onConnected);
   
   function onConnected()
   {
      diaphneaMap.getMapIds(function(mapIds)
      {
         console.log(mapIds);
      });
        
      var canvasId = 'canvas';
      var mapId = '_Gabon';
      var map = new diaphneaMap.Map(mapId, canvasId);
      
      map.getMapName('fr', function(mapName)
      {
         console.log(mapName);
      });
   }
});
