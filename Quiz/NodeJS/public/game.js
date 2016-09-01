$(function()
{
   var socket = io.connect();
   
   $('#nextButton').click(function(e)
   {
      if (displayedQuestion < questionCount - 1)
      {
         $('#question' + displayedQuestion).css('display', 'none');
         ++displayedQuestion;
         questionChange(displayedQuestion - 1);
      }
   });
   
   $('#previousButton').click(function(e)
   {
      if (displayedQuestion > 0)
      {
         $('#question' + displayedQuestion).css('display', 'none');
         --displayedQuestion;
         questionChange(displayedQuestion + 1);  
      }
   });
   
   function questionChange(previousQuestion)
   {
      $('#iQuestion').text(displayedQuestion + 1);
      $('#question' + displayedQuestion).css('display', 'block');
      
      if (displayedQuestion == 0) $('#previousButton').attr('disabled', 'disabled');
      else if (previousQuestion == 0) $('#previousButton').removeAttr('disabled');
      
      if (displayedQuestion == questionCount - 1 || !answered[displayedQuestion]) $('#nextButton').attr('disabled', 'disabled');
      else $('#nextButton').removeAttr('disabled');
      
      if (answered[displayedQuestion]) $('#submitButton').attr('disabled', 'disabled');
      else $('#submitButton').removeAttr('disabled');
      
      var data = 
      {
         quizId: quizId,
         displayedQuestion: displayedQuestion
      };
      
      socket.emit('changeQuestion', data);
   }
   
   $('#submitButton').click(function(e)
   {
      $('#submitButton').attr('disabled', 'disabled');
      $('.input' + displayedQuestion).attr('disabled', 'disabled');
      if (displayedQuestion != questionCount - 1) $('#nextButton').removeAttr('disabled');
      answered[displayedQuestion] = 1;
      
      var data = 
      {
         quizId: quizId,
         question: displayedQuestion,
         checks: []
      };
      
      var i, n = $('.input' + displayedQuestion).length;
      for (i = 0; i < n; ++i) data.checks.push($('#input' + displayedQuestion + '_' + i).is(':checked'));
      
      console.log('#wait' + displayedQuestion);
      $('#wait' + displayedQuestion).css('display', 'inline');
      socket.emit('submit', data);
   });
   
   socket.on('reload', function()
   {
      location.replace('/');
   });
   
   socket.on('updateQuestions', function(data)
   {      
      if (data.quizId != quizId)
      {
         location.replace('/');
      }
      else
      {
         data.questionStates.forEach(function(state)
         {
            var i = state.index;
            state.choiceStates.forEach(function(choice, j)
            {
               var id = '#input' + i + '_' + j;
               $(id).attr('disabled', 'disabled');
               answered[i] = 1;
               if (i == displayedQuestion) $('#submitButton').attr('disabled', 'disabled');
               
               var isChecked = Boolean(choice & 1);
               if (isChecked) $(id).prop('checked', true);
               else $(id).removeAttr('checked');
                           
               var isRight = Boolean(choice & 2);
               $(id).next().removeClass();
               if (isRight) $(id).next().addClass('boldChoice');
               
               var isMultiple = ($(id).prop('type') == 'checkbox');
               if (isMultiple)
               {
                  if (isChecked == isRight) $(id).next().addClass('greenChoice');
                  else $(id).next().addClass('redChoice');
               }
               else if (isChecked)
               {
                  if (isRight) $(id).next().addClass('greenChoice');
                  else $(id).next().addClass('redChoice');
               }
            });
         });
         
         $('.waitImg').css('display', 'none');
      }
   });
});
