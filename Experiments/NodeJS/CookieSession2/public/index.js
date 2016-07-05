$(function()
{
   var socket = io.connect();
   
   $('#button').click(function()
   {
      socket.emit('click', 'Click...'); 
   })
})