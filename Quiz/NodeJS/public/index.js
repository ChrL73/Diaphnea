$(function()
{
   var socket = io.connect();
   
   console.log("test...");
   
   $("#questionnaireSelect").change(function()
   {
      console.log($(this).val());
   })
})