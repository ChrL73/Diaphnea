$(function()
{
   $('#serverStatus').css('color', 'rgb(128, 0, 128)').css('font-weight', 'bold');
   $('#serverWidth').css('color', 'rgb(128, 0, 128)').css('font-weight', 'bold');
   $('#serverHeight').css('color', 'rgb(128, 0, 128)').css('font-weight', 'bold');
   $('#serverLeft').css('color', 'rgb(128, 0, 128)').css('font-weight', 'bold');
   $('#serverBottom').css('color', 'rgb(128, 0, 128)').css('font-weight', 'bold');
   
   var canvas  = document.querySelector('#canvas');
   var context = canvas.getContext('2d');
   context.textBaseline = 'bottom';
   
   $('form').submit(function(e)
   {
      var text = $('#text').val();
      var fontSize = $('#fontSize').val();
      var fontFamily = $('#fontFamily').val();
      
      $.post('http://localhost:3002', $('form').serialize(), function(data)
      {
         $('#serverStatus').html(data.message);
         
         context.clearRect(0, 0, canvas.width, canvas.height);
         
         if (data.message == 'OK')
         {
            $('#serverWidth').html(data.width);
            $('#serverHeight').html(data.height);
            $('#serverLeft').html(data.left);
            $('#serverBottom').html(data.bottom);
            
            var ws = data.width;
            var hs = data.height;
            var ls = data.left;
            var bs = data.bottom;
            var x = 10;
            var y = 10 + hs + bs;

            context.fillStyle = 'rgb(255, 128, 255)';
            context.fillRect(x + ls, y - hs - bs, ws, hs);

            context.fillStyle = 'rgb(0, 0, 0)';
            context.font = fontSize + 'px ' + fontFamily;
            context.fillText(text, x, y);
         }
         else
         {
            $('#serverWidth').html('');
            $('#serverHeight').html('');
            $('#serverLeft').html('');
            $('#serverBottom').html('');
         }
      });
      
      e.preventDefault();
   });
});
