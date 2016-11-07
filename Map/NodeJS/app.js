var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');


var messageNames =
[
   'mapIds', // 0
   'mapInfo', // 1
   'elementInfo', // 2
   'elementsInfo', // 3
   'itemData', // 4
   'look', // 5
   'render' // 6
];

var messageTypes = {};
messageNames.forEach(function(name, i)
{
   messageTypes[name] = i.toString();
});

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   socket.on('mapIdsReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.mapIds);
   });
   
   socket.on('mapInfoReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.mapInfo + ' ' + request.mapId);
   });
   
   socket.on('elementInfoReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.elementInfo + ' ' + request.mapId + ' ' + request.elementId);
   });
   
   socket.on('elementsInfoReq', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.elementsInfo + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
   });
   
   socket.on('itemDataReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.itemData + ' ' + request.mapId + ' ' + request.itemId + ' ' + request.resolution);
   });
   
   socket.on('lookReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.look + ' ' + request.mapId + ' ' + request.lookId);
   });
   
   socket.on('renderReq', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.render + ' ' + request.mapId + ' ' + request.width + ' ' + request.height;
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
   var messageName = messageNames[requestType];
   
   if (socket && messageName)
   {
      var response = { requestId: requestId, content: responseContent };
      socket.emit(messageName + 'Res', response);
   }
});

console.log('Map server listening on port 3001...');
server.listen(3001);
