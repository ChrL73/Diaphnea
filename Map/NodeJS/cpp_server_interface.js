var childProcess = require('child_process');
var readLine = require('readline');

var cppProcess;
var sendResponse;

function setResponseHandler(responseHandler)
{
   sendResponse = responseHandler;
}

function sendRequest(request, recursiveCall)
{
   if (!recursiveCall) console.log('Request: ' + request);
   
   try
   {
      cppProcess.stdin.write(request + '\n');
   }
   catch (err)
   {
      cppProcess = childProcess.spawn('./map_server.exe');
      
      readLine.createInterface(cppProcess.stdout, cppProcess.stdin).on('line', function(response)
      {
         console.log('Response: ' + response);

         var i = response.indexOf(' ');
         var socketId = response.substring(0, i);
         response = response.substring(i + 1);
         i = response.indexOf(' ');
         var requestId = Number(response.substring(0, i));
         response = response.substring(i + 1);
         i = response.indexOf(' ');
         var requestType = response.substring(0, i);
         var responseContent = JSON.parse(response.substring(i + 1));
         
         sendResponse(socketId, requestId, requestType, responseContent);
      });
      
      if (recursiveCall) throw new Error(err); // Todo: handle error
      sendRequest(request, true);
   }
}

module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
