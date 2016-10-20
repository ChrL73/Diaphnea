var childProcess = require('child_process');

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
      
      cppProcess.stdout.on('data', function(data)
      {
         var dataArray = data.toString().split('\n');  
         var responseArray = [];
         dataArray.forEach(function(s) { if (s.length > 0) responseArray.push(s); });
               
         responseArray.forEach(function(response)
         {
            console.log('Response: ' + response);
            var tokenArray = response.split(' ');
            sendResponse(tokenArray[0], Number(tokenArray[1]), tokenArray[2], JSON.parse(tokenArray[3]));
         });
      });
      
      if (recursiveCall) throw new Error(err); // Todo: hande error
      sendRequest(request, true);
   }
}

module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
