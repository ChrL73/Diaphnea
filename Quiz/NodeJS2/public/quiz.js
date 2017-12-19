$(function()
{
   var socket = io.connect();
   
   if (pageInfo.page == 'signUp') displaySignUp();
   else if (pageInfo.page == 'game') displayGame();
   else displayIndex();
   
   function displayIndex()
   {
      $('#container').empty();
      $('#container').removeClass();
      $('#container').addClass('container');
      
      var html = '<header><div class="navbar"><div class="col-md-10">';
      
      if (pageInfo.userName)
      {
         html += '<form class="navbar-form"><label>'
            + pageInfo.userName
            + '</label> <button id="indexSignOutBtn" class="btn btn-primary btn-sm">'
            + pageInfo.texts.signOut
            + '</button><div class="col-sm-1 navbar-right"><img src="wait.gif" id="indexNavBarWait" class="waitImg"/></div></form>'
      }
      else
      {
         html += '<form class="navbar-form"><small><label for="indexNameInput">'             
            + pageInfo.texts.name
            + ': </label> <input class="form-control input-sm" type="text" id="indexNameInput" /> <label for="indexPassInput">'
            + pageInfo.texts.password
            + ': </label> <input class="form-control input-sm" type="password" id="indexPassInput" /> <div class="visible-xs"><br></div><button id="indexSignInBtn" class="btn btn-primary btn-sm">'
            + pageInfo.texts.signIn
            + '</button> <button id="indexSignUpBtn" class="btn btn-info btn-sm">'
            + pageInfo.texts.signUp
            + '</button><div class="col-sm-1 navbar-right"><img src="wait.gif" id="indexNavBarWait" class="waitImg"/></div></small></form>';
      }
      
      html += '</div><div class="col-md-2 text-center"><div class="visible-xs visible-sm"><br></div>'
            + '<form class="navbar-form">'
            + '<select class="form-control input-sm" id="indexSiteLanguageSelect">';
      
      pageInfo.siteLanguageList.forEach(function(language)
      { 
         html += '<option ';
         if (language.id == pageInfo.siteLanguageId) html += 'selected ';
         html += 'value="' + language.id + '">' + language.name + '</option>';
      });
   
      html += '</select></form></div></div></header>';
      
      html += '<div class="modal fade" id="indexErrorMessage1"><div class="modal-dialog"><div class="modal-content"><div class="modal-body bg-danger">'
         + pageInfo.texts.unknownUserOrWrongPassword
         + '<button type="button" class="close" data-dismiss="modal">x</button></div></div></div></div> <div class="modal fade" id="indexErrorMessage2"><div class="modal-dialog"><div class="modal-content"><div class="modal-body bg-danger">'
         + pageInfo.texts.internalServerError
         + '<button type="button" class="close" data-dismiss="modal">x</button></div></div></div></div>';
      
      html += '<form><div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label" for="indexQuestionnaireSelect">'
            + pageInfo.texts.questionnaire
            + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control" id="indexQuestionnaireSelect">';
      
      pageInfo.questionnaireList.forEach(function(questionnaire)
      {
         html += '<option ';
         if (questionnaire.id == pageInfo.questionnaireId) html += 'selected ';
         html += 'value="' + questionnaire.id + '">' + questionnaire.name + '</option>';
      });
                                                                       
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexQuestionnaireWait" class="waitImg"/></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label questionnaireLanguageSelection" for="indexQuestionnaireLanguageSelect" style="margin-top:16px;">'
         + pageInfo.texts.language
         + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control questionnaireLanguageSelection" id="indexQuestionnaireLanguageSelect">';
      
      pageInfo.questionnaireLanguageList.forEach(function(language)
      { 
         html += '<option ';
         if (language.id == pageInfo.questionnaireLanguageId) html += 'selected ';
         html += 'value="' + language.id + '">' + language.name + '</option>';
      });
                                                                          
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexQuestionnaireLanguageWait" class="waitImg"/></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label" for="indexLevelSelect" style="margin-top:16px;">'
         + pageInfo.texts.level
         + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control" id="indexLevelSelect">';
      
      pageInfo.levelList.forEach(function(level)
      { 
         html += '<option ';
         if (level.id == pageInfo.levelId) html += 'selected ';
         html += 'value="' + level.id + '">' + level.name + '</option>';
      });
                      
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexLevelWait" class="waitImg"/></div></div>';
      
      html += '<div class="row" style="margin-top:20px;"><div class="col-sm-4 col-sm-offset-4"><button id="indexStartBtn" class="btn btn-success">'
         + pageInfo.texts.start
         + '</button></div><div class="col-sm-1"><img src="wait.gif" id="indexStartWait" class="waitImg"/></div></div>';
      
      html += '</form>';
      
      $('#container').append(html);
      
      $('#indexSignInBtn').click(function(e)
      {
         e.preventDefault();
         console.log('indexSignInBtn');
      });
      
      $('#indexSignUpBtn').click(function(e)
      {
         e.preventDefault();
         console.log('indexSignUpBtn');
      });
      
      $('#indexSignOutBtn').click(function(e)
      {
         e.preventDefault();
         console.log('indexSignOutBtn');
      });
      
      $('#indexStartBtn').click(function(e)
      {
         e.preventDefault();
         console.log('indexStartBtn');
      });
      
      if (pageInfo.unknown) $('#indexErrorMessage1').modal('show');
      if (pageInfo.error) $('#indexErrorMessage2').modal('show');
      
      if (!pageInfo.showQuestionnaireLanguageSelect) $('.questionnaireLanguageSelection').hide();
      
      $('#indexQuestionnaireSelect').change(function()
      {
         emitLevelChoice();
         $('#indexQuestionnaireWait').show();
      });

      $('#indexQuestionnaireLanguageSelect').change(function()
      {
         emitLevelChoice();
         $('#indexQuestionnaireLanguageWait').show();
      });

      $('#indexLevelSelect').change(function()
      {
         emitLevelChoice();
         $('#indexLevelWait').show();
      });

      function emitLevelChoice()
      {
         var questionnaireId = $('#indexQuestionnaireSelect').val();
         var questionnaireLanguageId = $('#indexQuestionnaireLanguageSelect').val();
         var levelId = $('#indexLevelSelect').val();

         if (!pageInfo.userName)
         {
            var expires = getCookieExpires(180);
            document.cookie = 'questionnaireId=' + questionnaireId + expires;
            document.cookie = 'questionnaireLanguageId=' + questionnaireLanguageId + expires;
            document.cookie = 'levelId=' + levelId + expires;
         }

         socket.emit('levelChoice', { questionnaireId: questionnaireId, questionnaireLanguageId: questionnaireLanguageId, levelId: levelId });
         
         function getCookieExpires(days)
         {
            var date = new Date();
            date.setTime(date.getTime() + days * 86400000);
            return "; expires=" + date.toGMTString();
         }
      }
      
      socket.on('updateSelects', function(data)
      {
         $('.waitImg').hide();

         $('#indexQuestionnaireSelect').find('option').remove(); 
         data.questionnaireList.forEach(function(questionnaire)
         {
            $('#indexQuestionnaireSelect').append('<option value="' + questionnaire.id + '">' + questionnaire.name + '</option>');
         });
         $('#indexQuestionnaireSelect').val(data.questionnaireId);

         if (data.showQuestionnaireLanguageSelect)
         {
            $('.questionnaireLanguageSelection').show();
         }
         else
         {
            $('.questionnaireLanguageSelection').hide();
         }

         $('#indexQuestionnaireLanguageSelect').find('option').remove();
         data.questionnaireLanguageList.forEach(function(language)
         {
            $('#indexQuestionnaireLanguageSelect').append('<option value="' + language.id + '">' + language.name + '</option>');
         });
         $('#indexQuestionnaireLanguageSelect').val(data.questionnaireLanguageId);

         $('#indexLevelSelect').find('option').remove();
         data.levelList.forEach(function(level)
         {
            $('#indexLevelSelect').append('<option value="' + level.id + '">' + level.name + '</option>');
         });
         $('#indexLevelSelect').val(data.levelId);
      });
   }
});
