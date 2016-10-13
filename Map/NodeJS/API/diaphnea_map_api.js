var diaphneaMap =
{
   connect: function(url, onConnected)
   {
      $.getScript(url + '/socket.io/socket.io.js', function()
      {         
         var socket = io(url);
         
         socket.emit('getMapIds');
         
         socket.on('mapIds', function(mapIds)
         {
            var maps = {};
            
            mapIds.forEach(function(id)
            {
               maps[id] = { id: id, socket: socket };
            });
            
            onConnected(maps);
         });
   
         
      });  
   }
}