$(function()
{
   var socket = io.connect();
   
   var date0 = Date.now();   
   var timeout = setTimeout(updateTime, 1000 * (1 + Math.floor(0.001 * t0)) - t0);
   
   function updateTime()
   {
      var t = (Date.now() - date0) + t0;
      var tSeconds = Math.floor(0.001 * t);
      $('#timeSpan').text(tSeconds);
      timeout = setTimeout(updateTime, 1000 * (1 + tSeconds) - t);
   }
   
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
         if (data.finalTime)
         {
            clearInterval(timeout);
            $('#timeSpan').text(data.finalTime);
         }
            
         data.questionStates.forEach(function(state)
         {
            var i = state.index;
            state.choiceStates.forEach(function(choiceState, j)
            {
               var id = '#input' + i + '_' + j;
               $(id).attr('disabled', 'disabled');
               answered[i] = 1;
               if (i == displayedQuestion) $('#submitButton').attr('disabled', 'disabled');
               
               var isChecked = Boolean(choiceState.state & 1);
               if (isChecked) $(id).prop('checked', true);
               else $(id).removeAttr('checked');
                           
               var isRight = Boolean(choiceState.state & 2);
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
               
               if (choiceState.comment)
               {
                  $('#comment' + i + '_' + j).text(' (' + choiceState.comment + ')');
               }
            });
         });
         
         $('#scoreSpan').text(data.rightAnswerCount + '/' + data.answerCount);
         
         $('.waitImg').css('display', 'none');
      }
   });
});
