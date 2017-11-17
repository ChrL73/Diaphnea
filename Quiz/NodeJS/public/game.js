$(function()
{
   resizeCanvas();
   
   var socket = io.connect();
   
   var map;
   var mapElements = {};
   mapServerInterface.createNewConnection(mapServerUrl, onConnected, onError);
   
   function onConnected(mapServerConnection)
   {
      mapServerConnection.loadMap(mapId, 'canvas', function(_map)
      {
         map = _map;
         updateMap(displayedQuestion);
         answered.forEach(function(b, i) { map.pushState(i); });
      });
   }
   
   function onError(error)
   {
      console.error(error);
   }
   
   var timeout;
   initTime(t0);
   socket.on('time', initTime);
   
   function initTime(t0)
   {
      $('#waitTimeImg').css('display', 'none');
      if (timeout) clearInterval(timeout);
      if (finished)
      {
         timeout = undefined;
         return;
      }
      
      var lastDisplayedTime;
      var date0 = Date.now();   
      timeout = setTimeout(updateTime, 1000 * (1 + Math.floor(0.001 * t0)) - t0);

      function updateTime()
      {
         var t = (Date.now() - date0) + t0;
         var displayedTime = Math.floor(0.001 * t);
         if (lastDisplayedTime && (displayedTime < lastDisplayedTime || displayedTime > lastDisplayedTime + 2))
         {
            socket.emit('timeRequest');
            timeout = undefined;
            $('#waitTimeImg').css('display', 'inline');
            return;
         }
         
         $('#timeSpan').text(displayedTime + 's');
         lastDisplayedTime = displayedTime;
         timeout = setTimeout(updateTime, 1000 * (1 + displayedTime) - t);
      }
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
      
      updateMap(displayedQuestion, previousQuestion);
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
   
   window.onresize = function()
   {
      resizeCanvas();
      if (map) map.redraw();
   };
   
   function resizeCanvas()
   {
      $('#canvasColumn').height((window.innerHeight - 72).toString() + 'px');
      
      $('#canvas').attr('width', $('#canvasColumn').width());
      $('#canvas').attr('height', window.innerHeight - 72);
   }
   
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
            finished = true;
            if (timeout) clearInterval(timeout);
            timeout = undefined;
            $('#timeSpan').text(data.finalTime + 's');
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
         
         $('.waitAnswerImg').css('display', 'none');
      }
   });
   
   function updateMap(newQuestion, oldQuestion)
   {
      if (!map) return;
      
      if (oldQuestion || oldQuestion === 0) map.pushState(oldQuestion);
      map.popState(newQuestion);
         
      mapIds[newQuestion].forEach(function(idInfo)
      {
         var element = mapElements[idInfo.id];
         if (element)
         {
            element.show();      
         }
         else
         {
            map.loadElement(idInfo.id, function(newElement)
            {
               mapElements[idInfo.id] = newElement;
               newElement.show();
            });
         }
      });
   }
});
