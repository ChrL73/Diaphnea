$(function()
{
   var socket = io.connect();
   
   socket.on('displayPage', function(data)
   {
      socket.removeAllListeners('updateSelects');
      socket.removeAllListeners('updateSignUp');
      
      if (data.page == 'signUp') displaySignUp(data);
      else if (data.page == 'game') displayGame(data);
      else displayIndex(data);
   });
   
   function displayIndex(pageData)
   {
      $('#container').empty();
      $('#container').removeClass();
      $('#container').addClass('container');
      
      var html = '<header><div class="navbar"><div class="col-md-10">';
      
      if (pageData.userName)
      {
         html += '<form class="navbar-form"><label>'
            + pageData.userName
            + '</label> <button id="indexSignOutBtn" class="btn btn-primary btn-sm">'
            + pageData.texts.signOut
            + '</button><div class="col-sm-1 navbar-right"><img src="wait.gif" id="indexNavBarWait" class="waitImg"/></div></form>'
      }
      else
      {
         html += '<form class="navbar-form"><small><label for="indexNameInput">'             
            + pageData.texts.name
            + ': </label> <input class="form-control input-sm" type="text" id="indexNameInput" value="'
            + pageData.tmpName
            + '"/> <label for="indexPassInput">'
            + pageData.texts.password
            + ': </label> <input class="form-control input-sm" type="password" id="indexPassInput" /> <div class="visible-xs"><br></div><button id="indexSignInBtn" class="btn btn-primary btn-sm">'
            + pageData.texts.signIn
            + '</button> <button id="indexSignUpBtn" class="btn btn-info btn-sm">'
            + pageData.texts.signUp
            + '</button><div class="col-sm-1 navbar-right"><img src="wait.gif" id="indexNavBarWait" class="waitImg"/></div></small></form>';
      }
      
      html += '</div><div class="col-md-2 text-center"><div class="visible-xs visible-sm"><br></div>'
            + '<form class="navbar-form">'
            + '<select class="form-control input-sm" id="indexSiteLanguageSelect">';
      
      pageData.siteLanguageList.forEach(function(language)
      { 
         html += '<option ';
         if (language.id == pageData.siteLanguageId) html += 'selected ';
         html += 'value="' + language.id + '">' + language.name + '</option>';
      });
   
      html += '</select></form></div></div></header>';
      
      html += '<div class="modal fade" id="indexErrorMessage1"><div class="modal-dialog"><div class="modal-content"><div class="modal-body bg-danger">'
         + pageData.texts.unknownUserOrWrongPassword
         + '<button type="button" class="close" data-dismiss="modal">x</button></div></div></div></div> <div class="modal fade" id="indexErrorMessage2"><div class="modal-dialog"><div class="modal-content"><div class="modal-body bg-danger">'
         + pageData.texts.internalServerError
         + '<button type="button" class="close" data-dismiss="modal">x</button></div></div></div></div>';
      
      html += '<form><div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label" for="indexQuestionnaireSelect">'
            + pageData.texts.questionnaire
            + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control" id="indexQuestionnaireSelect">';
      
      pageData.questionnaireList.forEach(function(questionnaire)
      {
         html += '<option ';
         if (questionnaire.id == pageData.questionnaireId) html += 'selected ';
         html += 'value="' + questionnaire.id + '">' + questionnaire.name + '</option>';
      });
                                                                       
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexQuestionnaireWait" class="waitImg waitImg1"/></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label questionnaireLanguageSelection" for="indexQuestionnaireLanguageSelect" style="margin-top:16px;">'
         + pageData.texts.language
         + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control questionnaireLanguageSelection" id="indexQuestionnaireLanguageSelect">';
      
      pageData.questionnaireLanguageList.forEach(function(language)
      { 
         html += '<option ';
         if (language.id == pageData.questionnaireLanguageId) html += 'selected ';
         html += 'value="' + language.id + '">' + language.name + '</option>';
      });
                                                                          
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexQuestionnaireLanguageWait" class="waitImg waitImg1"/></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><label class="control-label" for="indexLevelSelect" style="margin-top:16px;">'
         + pageData.texts.level
         + ': </label></div></div>';
      
      html += '<div class="row"><div class="col-sm-4 col-sm-offset-4"><select class="form-control" id="indexLevelSelect">';
      
      pageData.levelList.forEach(function(level)
      { 
         html += '<option ';
         if (level.id == pageData.levelId) html += 'selected ';
         html += 'value="' + level.id + '">' + level.name + '</option>';
      });
                      
      html += '</select></div><div class="col-sm-1"><img src="wait.gif" id="indexLevelWait" class="waitImg waitImg1"/></div></div>';
      
      html += '<div class="row" style="margin-top:20px;"><div class="col-sm-4 col-sm-offset-4"><button id="indexStartBtn" class="btn btn-success">'
         + pageData.texts.start
         + '</button></div><div class="col-sm-1"><img src="wait.gif" id="indexStartWait" class="waitImg"/></div></div>';
      
      html += '</form>';
      
      $('#container').append(html);
      
      $('#indexSignInBtn').click(function(e)
      {
         e.preventDefault();
         
      });
      
      $('#indexSignUpBtn').click(function(e)
      {
         e.preventDefault();
         var name = $('#indexNameInput').val();
         socket.emit('signUp', { name: name });
         $('#indexNavBarWait').show();
      });
      
      $('#indexSignOutBtn').click(function(e)
      {
         e.preventDefault();
         
      });
      
      $('#indexStartBtn').click(function(e)
      {
         e.preventDefault();
         
      });
      
      if (pageData.unknown) $('#indexErrorMessage1').modal('show');
      if (pageData.error) $('#indexErrorMessage2').modal('show');
      
      if (!pageData.showQuestionnaireLanguageSelect) $('.questionnaireLanguageSelection').hide();
      
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
      
      $('#indexSiteLanguageSelect').change(function()
      {
         var languageId = $(this).val();
         if (!pageData.userName) document.cookie = 'siteLanguageId=' + languageId + getCookieExpires(180);
         socket.emit('languageChoice', { page: 'index', languageId: languageId });
         $('#indexNavBarWait').show();
      });

      function emitLevelChoice()
      {
         var questionnaireId = $('#indexQuestionnaireSelect').val();
         var questionnaireLanguageId = $('#indexQuestionnaireLanguageSelect').val();
         var levelId = $('#indexLevelSelect').val();

         if (!pageData.userName)
         {
            var expires = getCookieExpires(180);
            document.cookie = 'questionnaireId=' + questionnaireId + expires;
            document.cookie = 'questionnaireLanguageId=' + questionnaireLanguageId + expires;
            document.cookie = 'levelId=' + levelId + expires;
         }

         socket.emit('levelChoice', { questionnaireId: questionnaireId, questionnaireLanguageId: questionnaireLanguageId, levelId: levelId });
      }
      
      socket.on('updateSelects', function(data)
      {
         console.log('test');
         
         $('.waitImg1').hide();

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
   
   function displaySignUp(pageData)
   {
      $('#container').empty();
      $('#container').removeClass();
      $('#container').addClass('container');
      
      var html = '<div><div class="col-md-4 col-md-offset-4 col-sm-offset-3 col-sm-6"><div class="form-group">'
         + '<header><div class="row"><div class="col-sm-8 col-sm-offset-2"><div class="navbar text-center"><form class="navbar-form"><select class="form-control input-sm" id="signUpSiteLanguageSelect">';
      
         pageData.siteLanguageList.forEach(function(language)
         { 
            html += '<option ';
            if (language.id == pageData.siteLanguageId) html += 'selected ';
            html += 'value="' + language.id + '">' + language.name + '</option>';
         });

         html += '</select></div></div><div class="col-sm-1 col-xs-1"><img src="wait.gif" id="signUpLanguageWait" class="waitImg"/></div></div></div></header>'
      
         + '<form><div class="form-group"><label for="signUpNameInput" id="signUpNameLabel">'
         + pageData.texts.name
         + ': </label><input class="form-control" type="text" id="signUpNameInput" value="'
         + pageData.name
         + '"/></div><div class="form-group"><label for="signUpPassInput1" id="signUpPassInput1Label">'
         + pageData.texts.password
         + ': </label> <input class="form-control" type="password" id="signUpPassInput1"/></div><div class="form-group"><label for="signUpPassInput2" id="signUpPassInput2Label">'
         + pageData.texts.confirmPassword
         + ': </label><input class="form-control" type="password" id="signUpPassInput2"/></div><div class="form-group"><div class="row"><div class="col-sm-10 col-xs-10"><button class="btn btn-info" id="submitSignUp">'
         + pageData.texts.signUp
         + '</button> <button class="btn btn-warning" id="cancelSignUp">'
         + pageData.texts.cancel
         + '</button></div><div class="col-sm-1 col-xs-1"><img src="wait.gif" id="signUpBtnWait" class="waitImg"/></div></div></div></div></form></div>';
      
      $('#container').append(html);
      
      $('#signUpSiteLanguageSelect').change(function()
      {
         var languageId = $(this).val();
         document.cookie = 'siteLanguageId=' + languageId + getCookieExpires(180);
         socket.emit('languageChoice', { page: 'signUp', languageId: languageId });
         $('#signUpLanguageWait').show();
      });
      
      $('#submitSignUp').click(function(e)
      {
         e.preventDefault();
         
      });
      
      $('#cancelSignUp').click(function(e)
      {
         e.preventDefault();
         var name = $('#signUpNameInput').val();
         socket.emit('index', {});
         $('#signUpBtnWait').show();
      });
      
      socket.on('updateSignUp', function(data)
      {
         $('#signUpLanguageWait').hide();
         $('#signUpNameLabel').text(data.name);
         $('#signUpPassInput1Label').text(data.password);
         $('#signUpPassInput2Label').text(data.confirmPassword);
         $('#submitSignUp').text(data.signUp);
         $('#cancelSignUp').text(data.cancel);
      });
   }
         
   function getCookieExpires(days)
   {
      var date = new Date();
      date.setTime(date.getTime() + days * 86400000);
      return "; expires=" + date.toGMTString();
   }
});
