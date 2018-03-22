var childProcess = require('child_process');
var readLine = require('readline');

var cppProcess;
var sendResponse;
var config;
var dbParameters;

function setConfig(config_)
{
   config = config_;
   
   var url = require('url');
   var dbParsedUrl = url.parse(config.dbUrl);
   dbParameters = [dbParsedUrl.host, dbParsedUrl.path.substr(1)];

   if (dbParsedUrl.auth)
   {
      var i = dbParsedUrl.auth.indexOf(':');
      if (i > -1)
      {
         dbParameters.push(dbParsedUrl.auth.substr(0, i));
         dbParameters.push(dbParsedUrl.auth.substr(i + 1));
      }
      else
      {
         dbParameters.push(dbParsedUrl.auth);
      }
   }
}

function setResponseHandler(responseHandler)
{
   sendResponse = responseHandler;
}

function sendRequest(request, recursiveCall)
{
   if (!recursiveCall && config.displayRequests) console.log('Request: ' + request);
   
   try
   {
      cppProcess.stdin.write(request + '\n');
   }
   catch (err)
   {
      if (!recursiveCall)
      {
         cppProcess = childProcess.spawn('./map_server.exe', dbParameters);
         
         // Don't crash on 'Connection reset' error
         cppProcess.stdin.on('error', function(err)
         {
            if (err.code != 'ECONNRESET') throw err; 
            else console.log("Warning: 'cppProcess.stdin' sent 'Connection reset' error");
         });

         readLine.createInterface(cppProcess.stdout, cppProcess.stdin).on('line', function(response)
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

         sendRequest(request, true);
      }
   }
}

module.exports.setConfig = setConfig;
module.exports.setResponseHandler = setResponseHandler;
module.exports.sendRequest = sendRequest;
