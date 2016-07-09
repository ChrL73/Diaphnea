$(function()
{
   $('.errorMessage').hide();
   if (reload) checkForm();
   console.log('reload: ' + reload);
   
   $('#siteLanguageSelect').change(function()
   {
      document.cookie = 'siteLanguageId=' + $(this).val() + getCookieExpires(180);
      location.reload();
   });
   
   $('form').submit(function(e)
   {
      /*if (!checkForm())
      {
         e.preventDefault();
      }*/
   });
    
   function checkForm()
   {
      var ok = true;
      
      var name = $('#nameInput').val();
      if (name.length < 2 || name.length > 16)
      {
         $(nameMessage).show();
         ok = false;
      }
      else
      {
         $(nameMessage).hide();
      }
      
      var pass1 = $('#passInput1').val();
      if (pass1.length >= 8)
      {
         $(pass1aMessage).hide();
         
         if (/^(?=.*[_,?;.:!$*+=&-])[A-Za-z0-9c_,?;.:!$*+=&-]+$/.test(pass1))
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
         $(pass1bMessage).hide();
         ok = false;
      }
      
      var pass2 = $('#passInput2').val();
      if (pass1 === pass2)
      {
         $(pass2Message).hide();
      }
      else
      {
         $(pass2Message).show();
         ok = false;
      }
      
      return ok;
   }
   
});
