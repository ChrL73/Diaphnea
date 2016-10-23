var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var requestTypes =
{
   getMapIds: '0',
   getMapInfo: '1',
   getElementInfo: '2',
   getElementsInfo: '3',
   render: '4'
};

var responseNames =
[
   'mapIds',
   'mapInfo',
   'elementInfo',
   'elementsInfo',
   'render'
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
   
   socket.on('getElementsInfo', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + requestTypes.getElementsInfo + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
   });
   
   socket.on('render', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + requestTypes.render + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
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
