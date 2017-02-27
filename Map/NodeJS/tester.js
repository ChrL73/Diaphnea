var log = {};

function initRecordMode()
{
   console.log("Replay data recorder on, enter 's' to stop recording and save data file...");
   var stdin = process.openStdin();
   stdin.on('data', onData);
   
   function onData(data)
   {
      if (data.toString('utf-8', 0, 1) == 's')
      {
         var fs = require('fs');
         fs.writeFile('log.txt', JSON.stringify(log), function(err)
         {
            if (err) console.log(err);
            else console.log('Log file saved');
         });
         
         record = undefined;
         stdin.removeListener('data', onData);
      }
   }
   
   module.exports.logRequest = function(socketId, requestType, request)
   {
      var key = socketId + '_' + request.id;
      
      if (!log[key]) log[key] = { requests: [], responses: [] };
      else console.log('Error: Several requests with socketId=' + socketId + ' and requestId=' + request.id);
      
      log[key].requests.push({ socketId: socketId, requestId: request.id, type: requestType, request: request });
   }
   
   module.exports.logResponse = function(socketId, requestId, requestType, responseContent)
   {
      var key = socketId + '_' + requestId;
      
      if (!log[key])
      {
         log[key] = { requests: [], responses: [] };
         console.log('Error: No request associated to the response with socketId=' + socketId + ' and requestId=' + request.id);
      }
      
      log[key].responses.push({ type: requestType, content: responseContent });
   }
}

function initReplayMode(onMapIdsReq, onMapInfoReq, onElementInfoReq, onElementsInfoReq, onItemDataReq, onLookReq, onRenderReq)
{
   console.log('Replay mode...');
   
   var fs = require('fs');
   fs.readFile('log.txt', 'utf-8', function(err, data)
   {
      if (err)
      {
         console.log(err);
      }
      else
      {
         log = JSON.parse(data);
         var logArray = [];
         expectedReplayRes = 0;

         Object.getOwnPropertyNames(log).forEach(function(key)
         {
            logArray.push(log[key].requests[0]);
            expectedReplayRes += log[key].responses.length;
         });

         var n = logArray.length;
         console.log(n + ' requests, ' + expectedReplayRes + ' expected responses');
         
         var date = new Date();
         var offset = date.getSeconds() * 1000 + date.getMilliseconds();
         
         var i;
         for (i = 0; i < n; ++i)
         {
            var j = (Math.floor(Math.random() * 1000000) + offset) % n;
            var tmp = logArray[i];
            logArray[i] = logArray[j];
            logArray[j] = tmp;
         }
         
         for (i = 0; i < n; ++i)
         {
            var request = logArray[i];

            if (request.type == '0') onMapIdsReq(request.socketId, request.request);
            if (request.type == '1') onMapInfoReq(request.socketId, request.request);
            if (request.type == '2') onElementInfoReq(request.socketId, request.request);
            if (request.type == '3') onElementsInfoReq(request.socketId, request.request);
            if (request.type == '4') onItemDataReq(request.socketId, request.request);
            if (request.type == '5') onLookReq(request.socketId, request.request);
            if (request.type == '6') onRenderReq(request.socketId, request.request);
         }
      }
   });
}

var receivedReplayRes = 0;
var expectedReplayRes;

function processReplayResponse(socketId, requestId, requestType, responseContent)
{
   var key = socketId + '_' + requestId;
      
   if (!log[key])
   {
      console.log('Error: Unexpected response id');
      process.exit();
   }
   else
   {
      var ok = false;
      log[key].responses.forEach(function(response)
      {
         if (requestType == '6' && response.content.items)
         {
            response.content.items.sort();
            responseContent.items.sort();
         }
         if (JSON.stringify(response) == JSON.stringify({ type: requestType, content: responseContent }))
         {
            ok = true;
            ++receivedReplayRes;
         }
      });

      if (!ok)
      {
         console.log('Error: Unexpected response content: ' + JSON.stringify({ type: requestType, content: responseContent }));
         console.log('Expected one of: ' + JSON.stringify(log[key].responses));
         process.exit();
      }

      if (receivedReplayRes == expectedReplayRes)
      {
         console.log('All expected responses have been received...');
      }
      else if (receivedReplayRes > expectedReplayRes)
      {
         console.log('Error: Too much responses have been received...');
      }
   }
}

module.exports.initRecordMode = initRecordMode;
module.exports.initReplayMode = initReplayMode;
module.exports.processReplayResponse = processReplayResponse;
