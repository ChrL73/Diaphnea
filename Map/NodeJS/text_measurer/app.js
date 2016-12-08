var Canvas = require('canvas');
var canvas = new Canvas(200, 200);
var ctx = canvas.getContext('2d');

var http = require('http');
var querystring = require('querystring');

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
         
         if (!text || !fontSize || !fontFamily || isNaN(fontSize) || fontSize < 1)
         {
            res.writeHead(200, { "Content-Type": "application/json", "Access-Control-Allow-Origin": "*" }); 
            res.end(JSON.stringify({ message: 'Bad request' }));
         }
         else
         {         
            ctx.font = fontSize + 'px ' + fontFamily;
            var measure = ctx.measureText(text);
            console.log(measure);
            
            var m = 
            {
               message: 'OK',
               width: measure.actualBoundingBoxRight + measure.actualBoundingBoxLeft,
               height: measure.actualBoundingBoxAscent + measure.actualBoundingBoxDescent,
               left: -measure.actualBoundingBoxLeft,
               bottom: measure.emHeightDescent - measure.actualBoundingBoxDescent - 1
            }

            console.log(m);
            
            res.writeHead(200, { "Content-Type": "application/json", "Access-Control-Allow-Origin": "*" }); 
            res.end(JSON.stringify(m));
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
