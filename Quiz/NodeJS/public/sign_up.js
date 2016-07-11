$(function()
{
   if (reload) checkForm();
   
   if (userExists) $('#nameMessage2').show();
   if (error) $('#serverErrorMessage').show();
   
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
      $('.errorMessage').hide();   
      var ok = true;
      
      var name = $('#nameInput').val();
      if (name.length < 2 || name.length > 16)
      {
         $('#nameMessage1').show();
         ok = false;
      }
      
      var pass1 = $('#passInput1').val();
      if (pass1.length >= 8)
      {
         if (!(/^(?=.*[_,?;.:!$*+=&-])[A-Za-z0-9c_,?;.:!$*+=&-]+$/.test(pass1)))
         {
            $('#pass1bMessage').show();
            ok = false;
         }
      }
      else
      {
         $('#pass1aMessage').show();
         ok = false;
      }
      
      var pass2 = $('#passInput2').val();
      if (pass1 !== pass2)
      {
         $('#pass2Message').show();
         ok = false;
      }
      
      return ok;
   }
   
});
