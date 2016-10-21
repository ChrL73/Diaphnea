var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var requestTypes =
{
   getMapIds: '0',
   getMapInfo: '1',
   getElementInfo: '2'
};

var responseNames =
[
   'mapIds',
   'mapInfo',
   'elementInfo'
];

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   socket.on('getMapIds', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapIds);
   });
   
   socket.on('getMapInfo', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapInfo + ' ' + request.mapId);
   });
   
   socket.on('getElementInfo', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getElementInfo + ' ' + request.mapId + ' ' + request.elementId);
   });
});

cppServer.setResponseHandler(function(socketId, requestId, requestType, responseContent)
{
   var messageName = responseNames[requestType];
   var socket = io.sockets.connected[socketId];
   
   if (messageName && socket)
   {
      var response = { requestId: requestId, content: responseContent };
      socket.emit(messageName, response);
   }
});

console.log('Map server listening on port 3001...');
server.listen(3001);
