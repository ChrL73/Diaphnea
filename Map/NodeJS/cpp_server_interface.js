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
         var array = data.toString().split('\n');  
         var array2 = [];
         array.forEach(function(s) { if (s.length > 0) array2.push(s); });
               
         console.log(array2.length.toString() + ' responses received:');
         array2.forEach(function(s) { console.log(s); });
      });
      
      if (recursiveCall) throw new Error(err); // Todo: hande error
      sendRequest(request, true);
   }
}

module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
