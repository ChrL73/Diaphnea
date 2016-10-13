var childProcess = require('child_process');

var child;
var sendResponse;

function setResponseHandler(responseHandler)
{
   sendResponse = responseHandler;
}

function sendRequest(request, recursiveCall)
{
   try
   {
      child.stdin.write(request + '\n');
   }
   catch (err)
   {
      child = childProcess.spawn('./map_server.exe');
      
      child.stdout.on('data', function(data)
      {
         var array = data.toString().split('\n');  
         console.log(array.length.toString() + ' responses received:');

         array.forEach(function(s)
         {
            console.log(s);
         });
      });
      
      if (recursiveCall) throw new Error(err); // Todo: hande error
      sendRequest(request, true);
   }
}

module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
