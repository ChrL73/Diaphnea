var http = require('http');
var server = http.createServer(function(req, res) { res.end(); });

var cppServer = require('./cpp_server_interface');
var config = require('./config');

var debugDelay = config.debugDelay ? config.debugDelay : 0;

var messageNames =
[
   'mapIds', // 0
   'mapInfo', // 1
   'elementInfo', // 2
   'elementsInfo', // 3
   'itemData', // 4
   'look', // 5
   'render', // 6
   'text' // 7
];

var messageTypes = {};
messageNames.forEach(function(name, i)
{
   messageTypes[name] = i.toString();
});

var io = require('socket.io').listen(server);

io.on('connection', function(socket)
{
   var received = 0, lastSent = 0, lastReceived = 0;
   socket.sent = 0;
   if (config.displayStat)
   {
      displayStat();
      setTimeout(displayStat, 1000);
   }
   
   function displayStat()
   {
      if (socket.sent != lastSent || received != lastReceived)
      {
         console.log(socket.id + ' Received: ' + received + ' sent: ' + socket.sent);
         lastSent = socket.sent;
         lastReceived = received;
      }
      
      setTimeout(displayStat, 1000);
   }
   
   socket.on('mapIdsReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.mapIds);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('mapInfoReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.mapInfo + ' ' + request.mapId);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('elementInfoReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.elementInfo + ' ' + request.mapId + ' ' + request.elementId);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('elementsInfoReq', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.elementsInfo + ' ' + request.mapId;
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('itemDataReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.itemData + ' ' + request.mapId + ' ' + request.itemId + ' ' + request.resolution);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('lookReq', function(request)
   {
      cppServer.sendRequest(socket.id + ' ' + request.id + ' ' + messageTypes.look + ' ' + request.mapId + ' ' + request.lookId);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
   
   socket.on('renderReq', function(request)
   {
      var req = socket.id + ' ' + request.id + ' ' + messageTypes.render + ' ' + request.mapId + ' ' + request.language + ' ' + request.width + ' ' + request.height + ' '
                + (request.scale ? request.scale : 'N') + ' ' + (request.xFocus ? request.xFocus : 'N') + ' ' + (request.yFocus ? request.yFocus : 'N');
      request.elementIds.forEach(function(elementId)
      {
         req += ' ' + elementId;
      });
      cppServer.sendRequest(req);
      if (config.displayStat) received += JSON.stringify(request).length;
   });
});

cppServer.setResponseHandler(function(socketId, requestId, requestType, responseContent)
{
   var socket = io.sockets.connected[socketId];
   var messageName = messageNames[requestType];
   
   if (socket && messageName)
   {
      var response = { requestId: requestId, content: responseContent };
      setTimeout(function()
      {
         socket.emit(messageName + 'Res', response);
         if (config.displayStat) socket.sent += JSON.stringify(response).length;
      },
         debugDelay);
   }
});

if (!config.port) throw new Error("No 'port' value in config.js");
console.log('Map server listening on port ' + config.port + '...');
server.listen(config.port);
