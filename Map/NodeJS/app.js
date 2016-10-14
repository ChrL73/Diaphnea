var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var requestTypes =
{
   getMapIds: '0',
   getMapName: '1'
};

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   console.log(socket.id);
   
   socket.on('getMapIds', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapIds);
   });
   
   socket.on('getMapName', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapName
                            + ' ' + request.mapId + ' ' + request.languageId);
   });
});

cppServer.setResponseHandler(function(socketId, requestId, requestType, response)
{
   //io.to(socketId).emit(...);
   //io.sockets.connected[socketId].emit(...);
});

console.log('Map server listening on port 3001...');
server.listen(3001);
