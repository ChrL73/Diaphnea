var config = require('./config');
var tester = require('./tester');

var record;
if (config.mode == 'record') record = true;

var replay;
if (config.mode == 'replay') replay = true;

var server;
var responses = {};

if (!replay)
{
   var http = require('http');
   var url = require('url');
   var querystring = require('querystring');
   var svgReqCounter = -1;
   
   server = http.createServer(function(req, res)
   {
      var route = url.parse(req.url).pathname;
      
      if (req.method == 'POST' && route == '/map.svg')
      {
         var reqData = '';

         req.on('data', function(data)
         {
            reqData += data;
         });

         req.on('end', function()
         {
            var body = querystring.parse(reqData);
            var data;
            
            try
            {
               data = JSON.parse(body.data);
            }
            catch (err)
            {
               data = undefined;
            }

            if (data)
            {
               data.id = ++svgReqCounter;
               responses[data.id] = res;
               onRenderReq('svg', data);
            }
            else
            {
               res.statusCode = 400;
               res.end();
            }
         });
      }
      else
      {
         res.statusCode = 404;
         res.end();
      }
   });
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
   'svg', //9
   'error', //10
];

var cppServer = require('./cpp_server_interface');
var svgMessageType = '9';
config.errorMessageIndex = 10;
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
   
   if (request.customColors)
   {
      req += ' #';
      request.customColors.forEach(function(customColor)
      {
         req += ' ' + customColor.i + ' ' + customColor.c + ' ' + customColor.o;
      });
   }
   
   cppServer.sendRequest(req);
}

cppServer.setResponseHandler(function(socketId, requestId, requestType, responseContent)
{
   if (record) tester.logResponse(socketId, requestId, requestType, responseContent);
   
   if (!replay)
   {
      if (socketId == 'svg')
      {
         res = responses[requestId];
         delete responses[requestId];
         if (res)
         {
            if (requestType == svgMessageType)
            {
               res.setHeader('content-type', 'image/svg+xml');
               res.setHeader('content-disposition', 'inline; filename=map.svg');
               res.end(responseContent.svg);
            }
            else
            {
               res.statusCode = 500;
               res.end();
            }
         }
      }
      else
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
