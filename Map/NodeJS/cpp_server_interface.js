/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

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
         //cppProcesses[mapId] = childProcess.spawn('./map_server2/bin/Debug/map_server2'); // Used for profiling
         
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
