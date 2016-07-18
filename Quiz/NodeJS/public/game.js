$(function()
{
   $('#siteLanguageSelect').change(function()
   {
      var languageId = $(this).val();
      if (!userLogged)
      {
         document.cookie = 'siteLanguageId=' + languageId + getCookieExpires(180);
         location.replace('/');
      }
      else
      {
         $('#siteLanguageForm').trigger('submit');
      }
   });
});
