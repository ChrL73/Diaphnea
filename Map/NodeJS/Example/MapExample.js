$(function()
{
   var url = 'http://192.168.50.56:3001';
   
   diaphneaMap.connect(url, onConnected);
   
   function onConnected(maps)
   {
      map = maps['_Gabon'];
   }
});