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
            ctx.font = fontSize + 'px sans-serif';
            var defaultMeasure = ctx.measureText(text);
            console.log('');
            //console.log(defaultMeasure);
            
            ctx.font = fontSize + 'px ' + fontFamily;
            var measure = ctx.measureText(text);
            console.log(measure);
            
            if (fontFamily.toLowerCase().trim() != 'arial' && fontFamily.toLowerCase().trim() != 'sans-serif' && JSON.stringify(measure) === JSON.stringify(defaultMeasure))
            {
               message = 'Unknown font';
            }
            else
            {
               message = 'OK';
            }
            
            var m = 
            {
               message: message,
               width: measure.actualBoundingBoxRight + measure.actualBoundingBoxLeft,
               height: measure.actualBoundingBoxAscent + measure.actualBoundingBoxDescent,
               left: -measure.actualBoundingBoxLeft,
               bottom: -measure.actualBoundingBoxDescent
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
