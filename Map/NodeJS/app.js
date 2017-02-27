var config = require('./config');
var tester = require('./tester');

var record;
if (config.mode == 'record') record = true;

var replay;
if (config.mode == 'replay') replay = true;

var server;
if (!replay)
{
   var http = require('http');
   server = http.createServer(function(req, res) { res.statusCode = 404; res.end(); });
}

var messageNames =
[
   'mapIds', // 0
   'mapInfo', // 1
   'elementInfo', // 2
   'elementsInfo', // 3
   'itemData', // 4
   'look', // 5
   'render', // 6
   'text', // 7
   'removeText', //8
   'error' //9
];

var cppServer = require('./cpp_server_interface');
config.errorMessageIndex = 9;
cppServer.setConfig(config);

var debugDelay = config.debugDelay ? config.debugDelay : 0;

var messageTypes = {};
messageNames.forEach(function(name, i)
{
   messageTypes[name] = i.toString();
});

var io;
if (!replay)
{
   io = require('socket.io').listen(server);

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
         onMapIdsReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('mapInfoReq', function(request)
      {
         onMapInfoReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('elementInfoReq', function(request)
      {
         onElementInfoReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('elementsInfoReq', function(request)
      {
         onElementsInfoReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('itemDataReq', function(request)
      {
         onItemDataReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('lookReq', function(request)
      {
         onLookReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
      
      socket.on('renderReq', function(request)
      {
         onRenderReq(socket.id, request);
         if (config.displayStat) received += JSON.stringify(request).length;
      });
   });
}

function onMapIdsReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.mapIds, request);
   cppServer.sendRequest(socketId + ' ' + request.id + ' ' + messageTypes.mapIds);
}

function onMapInfoReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.mapInfo, request);
   cppServer.sendRequest(socketId + ' ' + request.id + ' ' + messageTypes.mapInfo + ' ' + request.mapId);
}

function onElementInfoReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.elementInfo, request);
   cppServer.sendRequest(socketId + ' ' + request.id + ' ' + messageTypes.elementInfo + ' ' + request.mapId + ' ' + request.elementId);
}

function onElementsInfoReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.elementsInfo, request);
   var req = socketId + ' ' + request.id + ' ' + messageTypes.elementsInfo + ' ' + request.mapId;
   request.elementIds.forEach(function(elementId)
   {
      req += ' ' + elementId;
   });
   cppServer.sendRequest(req);
}

function onItemDataReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.itemData, request);
   cppServer.sendRequest(socketId + ' ' + request.id + ' ' + messageTypes.itemData + ' ' + request.mapId + ' ' + request.itemId + ' ' + request.resolution);
}

function onLookReq(socketId, request)
{
   if (record) tester.logRequest(socketId, messageTypes.look, request);
   cppServer.sendRequest(socketId + ' ' + request.id + ' ' + messageTypes.look + ' ' + request.mapId + ' ' + request.lookId);
}

function onRenderReq(socketId, request)
{
   if (record)
   {
      request.elementIds.push('#test');
      tester.logRequest(socketId, messageTypes.render, request);
   }
   
   var req = socketId + ' ' + request.id + ' ' + messageTypes.render + ' ' + request.mapId + ' ' + request.language + ' ' + request.width + ' ' + request.height + ' '
             + request.lookIndex + ' ' + (request.scale ? request.scale : 'N') + ' ' + (request.xFocus ? request.xFocus : 'N') + ' ' + (request.yFocus ? request.yFocus : 'N');
   request.elementIds.forEach(function(elementId)
   {
      req += ' ' + elementId;
   });
   cppServer.sendRequest(req);
}

cppServer.setResponseHandler(function(socketId, requestId, requestType, responseContent)
{
   if (record) tester.logResponse(socketId, requestId, requestType, responseContent);
   
   if (!replay)
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
   }
   else
   {
      tester.processReplayResponse(socketId, requestId, requestType, responseContent);
   }
});

if (record) tester.initRecordMode();
else if (replay) tester.initReplayMode(onMapIdsReq, onMapInfoReq, onElementInfoReq, onElementsInfoReq, onItemDataReq, onLookReq, onRenderReq);

if (!replay)
{
   if (!config.port) throw new Error("No 'port' value in config.js");
   console.log('Map server listening on port ' + config.port + '...');
   server.listen(config.port);
}
