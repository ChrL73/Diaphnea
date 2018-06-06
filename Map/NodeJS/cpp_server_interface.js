var childProcess = require('child_process');
var readLine = require('readline');
var mapIds = require('./map_ids');

var cppProcesses = {};
var sendResponse;
var config;

function setConfig(config_)
{
   config = config_;
}

function setResponseHandler(responseHandler)
{
   sendResponse = responseHandler;
}

function sendRequest(request, mapId, recursiveCall)
{
   if (!mapIds.idObject[mapId]) return;
   
   if (!recursiveCall && config.displayRequests) console.log('Request: ' + request);
   
   try
   {
      cppProcesses[mapId].stdin.write(request + '\n');
   }
   catch (err)
   {
      if (!recursiveCall)
      {
         cppProcesses[mapId] = childProcess.spawn('./map_server' + mapId + '.exe');
         
         // Don't crash on 'Connection reset' error
         cppProcesses[mapId].stdin.on('error', function(err)
         {
            if (err.code != 'ECONNRESET') throw err; 
            else console.log("Warning: 'cppProcesses[mapId].stdin' sent 'Connection reset' error");
         });

         readLine.createInterface(cppProcesses[mapId].stdout, cppProcesses[mapId].stdin).on('line', function(response)
         {
            var i = response.indexOf(' ');
            var socketId = response.substring(0, i);
            response = response.substring(i + 1);
            i = response.indexOf(' ');
            var requestId = Number(response.substring(0, i));
            response = response.substring(i + 1);
            i = response.indexOf(' ');
            var requestType = response.substring(0, i);
            var responseContent = JSON.parse(response.substring(i + 1));
            
            if (config.displayReponses || requestType == config.errorMessageIndex) console.log('Response: ' + response);

            sendResponse(socketId, requestId, requestType, responseContent);
         });

         sendRequest(request, mapId ,true);
      }
   }
}

module.exports.setConfig = setConfig;
module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
