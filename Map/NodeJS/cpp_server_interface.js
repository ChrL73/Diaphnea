var childProcess = require('child_process');

var cppProcess;
var sendResponse;

function setResponseHandler(responseHandler)
{
   sendResponse = responseHandler;
}

function sendRequest(request, recursiveCall)
{
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
               
         //console.log(array2.length.toString() + ' responses received:');
         responseArray.forEach(function(response)
         {
            var tokenArray = response.split(' ');
            var tokenSubArray = [];
            tokenArray.forEach(function(token, i)
            {
               if (i > 2) tokenSubArray.push(token);
            });
            
            sendResponse(tokenArray[0], Number(tokenArray[1]), tokenArray[2], tokenSubArray);
            //console.log(response);
         });
      });
      
      if (recursiveCall) throw new Error(err); // Todo: hande error
      sendRequest(request, true);
   }
}

module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
