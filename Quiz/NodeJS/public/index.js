$(function()
{
   var socket = io.connect();
   
   console.log('test...');
   
   $('#questionnaireSelect').change(function()
   {
      socket.emit('questionnaireChoice', $(this).val()); 
   })
})