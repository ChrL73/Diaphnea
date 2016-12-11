var http = require('http');
var querystring = require('querystring');
var childProcess = require('child_process');

var server = http.createServer(function(req, res)
{
   if (req.method == 'POST')
   {
      var reqData = '';
      
      req.on('data', function(data)
      {
         reqData += data;
      });

      req.on('end', function()
      {
         var body = querystring.parse(reqData);
         
         var text = body.text;
         var fontSize = body.fontSize;
         var fontFamily = body.fontFamily;
         var message;
         
         if (!text) message = 'No text';
         else if (!fontSize) message = 'No font size';
         else if (isNaN(fontSize) || fontSize < 1 || fontSize > 999)  message = 'Bad font size';
         else if (!fontFamily) message = 'No font family';
         
         if (message)
         {
            res.writeHead(200, { "Content-Type": "application/json", "Access-Control-Allow-Origin": "*" }); 
            res.end(JSON.stringify({ message: message }));
         }
         else
         {         
            var response = { message: 'Not implemented'};
            
            childProcess.exec('.\\..\\..\\..\\Map\\VisualStudio\\Debug\\FreeTypeExperiment2.exe', function(err, stdout, stderr)
            {
               if (err) console.log('err: ' + err);
               if (stdout) console.log('stdout: ' + stdout);
               if (stderr) console.log('stderr: ' + stderr);
            });

            
            res.writeHead(200, { "Content-Type": "application/json", "Access-Control-Allow-Origin": "*" }); 
            res.end(JSON.stringify(response));
         }
      });
   }
   else
   {
      res.writeHead(404);
      res.end('Not found');
   }
});

server.listen(3002);
console.log('Text measurer server listening on port 3002...');
