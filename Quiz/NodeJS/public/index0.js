$(function()
{
   var socket = io.connect();
   
   if (unknwon) $('#cnxMessage1').show();
   if (error) $('#cnxMessage2').show();
   
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
   
   $('#questionnaireSelect').change(emitLevelChoice);
   $('#languageSelect').change(emitLevelChoice);
   $('#levelSelect').change(emitLevelChoice);
   
   function emitLevelChoice()
   {
      var questionnaireId = $('#questionnaireSelect').val();
      var languageId = $('#languageSelect').val();
      var levelId = $('#levelSelect').val();
      
      if (!userLogged)
      {
         var expires = getCookieExpires(180);
         document.cookie = 'questionnaireId=' + questionnaireId + expires;
         document.cookie = 'languageId=' + languageId + expires;
         document.cookie = 'levelId=' + levelId + expires;
      }
      
      socket.emit('levelChoice', { questionnaireId: questionnaireId, languageId: languageId, levelId: levelId }); 
   }
   
   socket.on('updateSelects', function(data)
   {
      $('#questionnaireSelect').find('option').remove(); 
      data.questionnaireList.forEach(function(questionnaire)
      {
         $('#questionnaireSelect').append('<option value="' + questionnaire.id + '">' + questionnaire.name + '</option>');
      });
      $('#questionnaireSelect').val(data.questionnaireId);
      
      if (data.showLanguageSelect)
      {
         $('.languageSelection').css('display', 'inline');
      }
      else
      {
         $('.languageSelection').css('display', 'none');
      }
      
      $('#languageSelect').find('option').remove();
      data.languageList.forEach(function(language)
      {
         $('#languageSelect').append('<option value="' + language.id + '">' + language.name + '</option>');
      });
      $('#languageSelect').val(data.languageId);
      
      $('#levelSelect').find('option').remove();
      data.levelList.forEach(function(level)
      {
         $('#levelSelect').append('<option value="' + level.id + '">' + level.name + '</option>');
      });
      $('#levelSelect').val(data.levelId);
   });
});
