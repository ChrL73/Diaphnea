$(function()
{
   var socket = io.connect();
   
   $('#nextButton').click(function(e)
   {
      if (displayedQuestion < questionCount - 1)
      {
         $('#question' + displayedQuestion).css('display', 'none');
         ++displayedQuestion;
         $('#iQuestion').text(displayedQuestion + 1);
         $('#question' + displayedQuestion).css('display', 'block');
         if (displayedQuestion == 1) $('#previousButton').removeAttr('disabled');
         if (displayedQuestion == questionCount - 1) $('#nextButton').attr('disabled', 'disabled'); 
         if (answered[displayedQuestion]) $('#submitButton').attr('disabled', 'disabled');
         else $('#submitButton').removeAttr('disabled');
         socket.emit('changeQuestion', displayedQuestion);
      }
   });
   
   $('#previousButton').click(function(e)
   {
      if (displayedQuestion > 0)
      {
         $('#question' + displayedQuestion).css('display', 'none');
         --displayedQuestion;
         $('#iQuestion').text(displayedQuestion + 1);
         $('#question' + displayedQuestion).css('display', 'block');
         if (displayedQuestion == questionCount - 2) $('#nextButton').removeAttr('disabled');
         if (displayedQuestion == 0) $('#previousButton').attr('disabled', 'disabled');
         if (answered[displayedQuestion]) $('#submitButton').attr('disabled', 'disabled');
         else $('#submitButton').removeAttr('disabled');
         socket.emit('changeQuestion', displayedQuestion);
      }
   });
   
   $('#submitButton').click(function(e)
   {
      $('#submitButton').attr('disabled', 'disabled');
      $('.input' + displayedQuestion).attr('disabled', 'disabled');
      answered[displayedQuestion] = 1;
      socket.emit('submit', displayedQuestion);
   });
});
