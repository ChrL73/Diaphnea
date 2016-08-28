$(function()
{
   $('#siteLanguageForm').append('<input type="hidden" id="nameInput2" name="name" value="" />');
   $('#siteLanguageForm').append('<input type="hidden" id="passInput1_2" name="pass1" value="" />');
   $('#siteLanguageForm').append('<input type="hidden" id="passInput2_2" name="pass2" value="" />');
   
   if (reload) checkForm();
   
   if (userExists) $('#nameMessage2').show();
   if (error) $('#serverErrorMessage').show();
   
   $('#siteLanguageSelect').change(function()
   {
      document.cookie = 'siteLanguageId=' + $(this).val() + getCookieExpires(180);
      $('#nameInput2').val($('#nameInput').val());
      $('#passInput1_2').val($('#passInput1').val());
      $('#passInput2_2').val($('#passInput2').val());
      $('#siteLanguageForm').trigger('submit');
   });
   
   var cancel = false;
   $('#cancelSignUp').click(function(e)
   {
      cancel = true;
   });
   
   $('#signUpForm').submit(function(e)
   {
      if (!cancel)
      {
         if (!checkForm())
         {
            e.preventDefault();
         }
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
