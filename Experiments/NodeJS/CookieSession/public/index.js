$(function()
{
   var socket = io.connect();
   
   $('#countrySelect').change(function()
   {
      var country = $(this).val();
      document.cookie = "country=" + country;
      socket.emit('countryChoice', country); 
   })
})