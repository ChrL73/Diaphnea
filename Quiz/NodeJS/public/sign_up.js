$(function()
{
   $('.errorMessage').hide();
   
   var socket = io.connect();
   
   $('#siteLanguageSelect').change(function()
   {
      document.cookie = 'siteLanguageId=' + $(this).val() + getCookieExpires(180);
      location.reload();
   });
   
   $('form').submit(function(e)
   {
      if (!checkForm())
      {
         e.preventDefault();
      }
   });
   
   
   function checkForm()
   {
      var ok = true;
      
      var val = $('#nameInput').val();
      if (val.length < 2 || val.length > 16)
      {
         $(nameMessage).show();
         ok = false;
      }
      else
      {
         $(nameMessage).hide();
      }
      
      val = $('#passInput1').val();
      if (val.length >= 8)
      {
         $(pass1aMessage).hide();
         
         if (/^[A-Za-z0-9_,?;.:!$*+-=&]+$/.test(val))
         {
            $(pass1bMessage).hide();
         }
         else
         {
            $(pass1bMessage).show();
            ok = false;
         }
      }
      else
      {
         $(pass1aMessage).show();
         ok = false;
      }
      
      
      
      return ok;
   }
   
});
