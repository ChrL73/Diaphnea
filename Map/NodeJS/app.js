var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var requestTypes =
{
   getMapIds: '0',
   getMapLanguages: '1',
   getMapName: '2',
   getElementIds: '3'
};

var responseNames =
[
   'mapIds',
   'mapLanguages',
   'mapName',
   'elementIds'
];

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   socket.on('getMapIds', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapIds);
   });
   
   socket.on('getMapLanguages', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapLanguages + ' ' + request.mapId);
   });
   
   socket.on('getMapName', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getMapName
                            + ' ' + request.mapId + ' ' + request.languageId);
   });
   
   socket.on('getElementIds', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + requestTypes.getElementIds + ' ' + request.mapId);
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
