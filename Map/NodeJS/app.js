var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var requestTypes =
{
   getMapIds: '0'
};

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   console.log(socket.id);
   
   socket.on('getMapIds', function()
   {
      cppServer.sendRequest(socket.id + ' ' + requestTypes.getMapIds);
   });
});

cppServer.setResponseHandler(function(socketId, requestType, response)
{
   //io.to(socketId).emit(...);
   //io.sockets.connected[socketId].emit(...);
});

console.log('Map server listening on port 3001...');
server.listen(3001);
