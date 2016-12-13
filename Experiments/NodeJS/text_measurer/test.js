$(function()
{
   var canvas1  = document.querySelector('#canvas1');
   var context1 = canvas1.getContext('2d');
   //context1.textBaseline = 'bottom';
   
   var canvas2 = document.querySelector('#canvas2');
   var context2 = canvas2.getContext('2d');
   //context2.textBaseline = 'bottom';
   
   $('#form1').submit(function(e)
   {
      var text = $('#text1').val();
      var fontSize = $('#fontSize1').val();
      var fontFamily = $('#fontFamily1').val();
      
      $.post('http://localhost:3002', $('#form1').serialize(), function(data)
      {
         $('#status1').html(data.message);
         
         context1.clearRect(0, 0, canvas1.width, canvas1.height);
         
         if (data.message == 'OK' || data.message == 'Unknown font')
         {
            $('#width1').html(data.width);
            $('#height1').html(data.height);
            $('#left1').html(data.left);
            $('#bottom1').html(data.bottom);
            
            var ws = data.width;
            var hs = data.height;
            var ls = data.left;
            var bs = data.bottom;
            var x = 10;
            var y = 10 + hs + bs;

            context1.fillStyle = 'rgb(255, 128, 255)';
            context1.fillRect(x + ls, y - hs - bs, ws, hs);

            context1.fillStyle = 'rgb(0, 0, 0)';
            context1.font = fontSize + 'px ' + fontFamily;
            context1.fillText(text, x, y);
         }
         else
         {
            $('#width1').html('');
            $('#height1').html('');
            $('#left1').html('');
            $('#bottom1').html('');
         }
      });
      
      e.preventDefault();
   });
   
   $('#form2').submit(function(e)
   {
      var text = $('#text2').val();
      var refFontSize = $('#refFontSize2').val();
      var fontSize = $('#fontSize2').val();
      var fontFamily = $('#fontFamily2').val();
      
      $.post('http://localhost:3002', $('#form2').serialize(), function(data)
      {
         $('#status2').html(data.message);
         
         context2.clearRect(0, 0, canvas2.width, canvas2.height);
         
         if (data.message == 'OK' || data.message == 'Unknown font')
         {
            $('#width2').html(data.width);
            $('#height2').html(data.height);
            $('#left2').html(data.left);
            $('#bottom2').html(data.bottom);
            
            var ws = data.width * fontSize / refFontSize;
            var hs = data.height * fontSize / refFontSize;
            var ls = data.left * fontSize / refFontSize;
            var bs = data.bottom * fontSize / refFontSize;
            var x = 10;
            var y = 10 + hs + bs;

            context2.fillStyle = 'rgb(255, 128, 255)';
            context2.fillRect(x + ls, y - hs - bs, ws, hs);

            context2.fillStyle = 'rgb(0, 0, 0)';
            context2.font = fontSize + 'px ' + fontFamily;
            context2.fillText(text, x, y);
         }
         else
         {
            $('#width2').html('');
            $('#height2').html('');
            $('#left2').html('');
            $('#bottom2').html('');
         }
      });
      
      e.preventDefault();
   });
});
