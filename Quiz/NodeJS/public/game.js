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
      
      if (displayedQuestion == questionCount - 1) $('#nextButton').attr('disabled', 'disabled');
      else if (previousQuestion == questionCount - 1) $('#nextButton').removeAttr('disabled');
      
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
      answered[displayedQuestion] = 1;
      
      var data = 
      {
         quizId: quizId,
         question: displayedQuestion,
         checks: []
      };
      
      var i, n = $('.input' + displayedQuestion).length;
      for (i = 0; i < n; ++i) data.checks.push($('#input' + displayedQuestion + '_' + i).is(':checked'));
      
      socket.emit('submit', data);
   });
   
   socket.on('reload', function()
   {
      location.replace('/');
   });
});
