var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');

var messageTypes =
{
   getMapIds: '0',
   getMapInfo: '1',
   getElementInfo: '2',
   getElementsInfo: '3',
   getItemInfo: '4',
   render: '5',
};

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   socket.on('getMapIds', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.getMapIds);
   });
   
   socket.on('getMapInfo', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.getMapInfo + ' ' + request.mapId);
   });
   
   socket.on('getElementInfo', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.getElementInfo + ' ' + request.mapId + ' ' + request.elementId);
   });
   
   socket.on('getElementsInfo', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.getElementsInfo + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
   });
   
   socket.on('getItemInfo', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.getItemInfo + ' ' + request.mapId + ' ' + request.itemId + ' ' + request.resolution);
   });
   
   socket.on('render', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.render + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
   });
});

cppServer.setResponseHandler(function(socketId, requestId, requestType, responseContent)
{
   var socket = io.sockets.connected[socketId];
   
   if (socket)
   {
      var messageName = (requestType == 0 ? 'mapIds' : 'res');
      var response = { requestId: requestId, content: responseContent };
      socket.emit(messageName, response);
   }
});

console.log('Map server listening on port 3001...');
server.listen(3001);
