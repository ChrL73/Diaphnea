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
            + ': </label><input class="form-control input-sm" type="text" id="nameInput" name="name"/><label for="passInput">'
            + pageInfo.texts.password
            + ': </label><input class="form-control input-sm" type="password" id="passInput" name="pass"/><div class="visible-xs"><br></div><input class="btn btn-primary btn-sm" type="submit" value="'
            + pageInfo.texts.signIn
            + '" name="signIn"/><input class="btn btn-info btn-sm" type="submit" value="'
            + pageInfo.texts.signUp
            + '" name="enterSignUp"/></small></form>';
      }
      
      html += '</div></div></header>';
      
      $('#container').append(html);
   }
}) ;
