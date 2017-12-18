$(function()
{
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
         
      }
      else
      {
         html += '<form class="navbar-form"><small><label for="nameInput">'             
            + pageInfo.texts.name
            + ': </label> <input class="form-control input-sm" type="text" id="indexNameInput" /> <label for="passInput">'
            + pageInfo.texts.password
            + ': </label> <input class="form-control input-sm" type="password" id="indexPassInput" /> <div class="visible-xs"><br></div><button id="indexSignInBtn" class="btn btn-primary btn-sm">'
            + pageInfo.texts.signIn
            + '</button> <button id="indexSignUpBtn" class="btn btn-info btn-sm">'
            + pageInfo.texts.signUp
            + '</button></small></form>';
      }
      
      html += '</div></div></header>';
      
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
   }
}) ;
