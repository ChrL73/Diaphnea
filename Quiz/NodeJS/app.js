var config = require('./config');
var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var session = require('express-session');
var mongoStore = require('connect-mongo')(session);
var favicon = require('serve-favicon');
var childProcess = require('child_process');
var shortId = require('shortid');

console.log('Test: ' + process.env.HEROKU_RELEASE_VERSION);

var mongoose = require('mongoose');
var db = mongoose.connect(config.dbUrl);

var url = require('url');
var dbParsedUrl = url.parse(config.dbUrl);
var dbParameters = ' ' + dbParsedUrl.host + ' ' + dbParsedUrl.path.substr(1);

if (dbParsedUrl.auth)
{
   var i = dbParsedUrl.auth.indexOf(':');
   if (i > -1)
   {
      dbParameters += ' ' + dbParsedUrl.auth.substr(0, i);
      dbParameters += ' ' + dbParsedUrl.auth.substr(i + 1);
   }
   else
   {
      dbParameters += ' ' + dbParsedUrl.auth;
   }
}

var quizData = require('./quiz_data');
var userData = require('./user_data');
var translate = require('./translate');
var languages = translate().languages;
var defaultLanguageId = languages[0].id;

app.use(express.static('public'));
app.use(morgan('dev'));
app.use(favicon('public/favicon.ico'));

if (!config.sessionSecret) throw new Error("No 'sessionSecret' value in config.js");
var sessionMiddleware = session(
{
   secret: config.sessionSecret,
   resave: false,
   saveUninitialized: true,
   store: new mongoStore(
   {
      mongooseConnection: mongoose.connection,
      ttl: 24 * 3600
   })
});
io.use(function(socket, next)
{
   sessionMiddleware(socket.request, socket.request.res, next);
});
app.use(sessionMiddleware);

var mapServerUrl;
if (config.mapServerAddress)
{
   mapServerUrl = config.mapServerProtocol + '://' + config.mapServerAddress;
}
else
{
   var locaIp;
   var os = require('os');
   var ifaces = os.networkInterfaces();
   Object.keys(ifaces).forEach(function(ifname)
   {
      if (!locaIp)
      {
         ifaces[ifname].forEach(function(iface)
         { 
            if (!locaIp && iface.family === 'IPv4' && iface.internal === false) 
            {
               locaIp = iface.address;
            }
         });
      }
   });
   mapServerUrl = config.mapServerProtocol + '://' + locaIp;
}
if (config.mapServerPort && config.mapServerPort !== 'none') mapServerUrl += ':' + config.mapServerPort;

var pages =
{
   index: 0,
   signUp: 1,
   game: 2,
};

var dTable = [1, 7, 30, 365];

var debugDelay = config.debugDelay ? config.debugDelay : 0;

quizData.getLevelMap(function(levelMap) { /*console.log(levelMap);*/ } );

app.all('/', function(req, res, next)
{
   if (config.forceHttpsRedirection && req.headers['x-forwarded-proto'] !== 'https') res.redirect(config.httpsUrl);
   else res.render(config.frontEndIndex, config);
});

app.use(function(req, res)
{
   if (config.forceHttpsRedirection && req.headers['x-forwarded-proto'] !== 'https') res.redirect(config.httpsUrl);
   else res.redirect('/');
});

io.on('connection', function(socket)
{
   var cookies = extractCookies(socket.handshake.headers.cookie);
   getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
   {
      if (context.currentPage == pages.signUp)
      {
         emitDisplaySignUp(context);
      }
      else if (context.currentPage == pages.game)
      {
         emitDisplayGame(context);
      }
      else
      {
         emitDisplayIndex(context);
      }
   });
         
   function emitDisplayIndex(context)
   {
      quizData.getLevelChoiceDownData(context, function(downData)
      {
         context.currentPage = pages.index;
         context.questionnaireId = downData.questionnaireId;
         context.questionnaireLanguageId = downData.questionnaireLanguageId;
         context.levelId = downData.levelId;
         context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });      

         var texts = translate(context.siteLanguageId).texts;

         downData.texts = getIndexTexts(texts);
         downData.page = 'index';
         if (context.user) downData.userName = context.user.name;
         else downData.tmpName = context.tmpName;
         downData.siteLanguageList = languages;
         downData.siteLanguageId = context.siteLanguageId;
         downData.scoreTab = context.scoreTab;
         downData.version = config.version;
         downData.sourceUrl = config.sourceUrl;
         downData.issueUrl = config.issueUrl;

         setTimeout(function() { socket.emit('displayPage', downData); }, debugDelay);   
      });
   }
   
   function getIndexTexts(texts)
   {
      var t = 
      {
         name: texts.name,
         password: texts.password,
         signIn: texts.signIn,
         signUp: texts.signUp,
         signOut: texts.signOut,
         unknownUserOrWrongPassword: texts.unknownUserOrWrongPassword,
         internalServerError: texts.internalServerError,
         questionnaire: texts.questionnaire,
         language: texts.language,
         level: texts.level,
         start: texts.start,
         highScores: texts.highScores,
         day: texts.day,
         week: texts.week,
         month: texts.month,
         year: texts.year,
         score: texts.score,
         time: texts.time,
         version: texts.version,
         sourceCode: texts.sourceCode,
         issues: texts.issues,
         warning: texts.warning,
         alphaWarningLine1: texts.alphaWarningLine1,
         alphaWarningLine2: texts.alphaWarningLine2,
         alphaWarningLine3: texts.alphaWarningLine3,
         alphaWarningLine4: texts.alphaWarningLine4,
         alphaWarningLine5: texts.alphaWarningLine5,
         alphaWarningLine6: texts.alphaWarningLine6,
         close: texts.close
      };
      
      return t;
   }
   
   function emitDisplaySignUp(context)
   {      
      context.currentPage = pages.signUp;
      context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
      
      var texts = translate(context.siteLanguageId).texts;
         
      var downData =
      {
         page: 'signUp',
         texts: getSignUpTexts(texts),
         name: context.tmpName,
         siteLanguageList: languages,
         siteLanguageId: context.siteLanguageId,
         name1Message: context.signUpMessages.name1,
         name2Message: context.signUpMessages.name2,
         errorMessage: context.signUpMessages.error
      };
      
      if (context.sendScoreWithSignUp)
      {
         downData.questionnaireName = context.questionnaireName;
         downData.levelName = context.levelName;
         downData.rightAnswerCount = context.rightAnswerCount;
         downData.finalTime = context.finalTime;
      }

      setTimeout(function() { socket.emit('displayPage', downData); }, debugDelay);
   }
   
   function getSignUpTexts(texts)
   {
      var t = 
      {
         name: texts.name,
         password: texts.password,
         confirmPassword: texts.confirmPassword,
         signUp: texts.signUp,
         cancel: texts.cancel,
         signIn: texts.signIn,
         nameMustBeBetween2And16Chars: texts.nameMustBeBetween2And16Chars,
         passwordMustContainAtLeast8Chars: texts.passwordMustContainAtLeast8Chars,
         passwordMustContainOnlyLettersNumbersEtc: texts.passwordMustContainOnlyLettersNumbersEtc,
         twoPasswordsAreNotIdentical: texts.twoPasswordsAreNotIdentical,
         theNameIsAlreadyUsed: texts.theNameIsAlreadyUsed,
         internalServerError: texts.internalServerError,
         yourScore: texts.yourScore,
         level: texts.level,
         questionnaire: texts.questionnaire,
         score: texts.score,
         time: texts.time,
         signUpProposal: texts.signUpProposal,
         unknownUserOrWrongPassword: texts.unknownUserOrWrongPassword
      };
      
      return t;
   }
   
   function emitDisplayGame(context)
   {
      context.currentPage = pages.game;
      context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
      
      var texts = translate(context.siteLanguageId).texts;
      
      var questions = [];
      var mapInfo = [];
      context.questions.forEach(function(question, iQuestion)
      {
         var sentQuestion =
         {
            question: question.question,
            isMultiple: question.isMultiple,
            choices: []
         };
         
         question.choices.forEach(function(choice)
         {
            var sentChoice = { text: choice.text };   
            if (context.questionStates[iQuestion].answered) sentChoice.comment = choice.comment;
            sentQuestion.choices.push(sentChoice);
         });
         
         questions.push(sentQuestion);
         
         if (context.questionStates[iQuestion].answered)
         {
            mapInfo.push(
            {
               mapIds: context.questions[iQuestion].mapIds,
               framingLevel: context.questions[iQuestion].framingLevel,
               mode: context.questions[iQuestion].mode,
               categories: context.questions[iQuestion].categories
            });
         }
      });        
         
      var downData =
      {
         page: 'game',
         texts: getGameTexts(texts),
         quizId: context.quizId,
         displayedQuestion: context.displayedQuestion,
         questions: questions,
         questionStates: context.questionStates,
         questionnaireName: context.questionnaireName,
         levelName: context.levelName,
         rightAnswerCount: context.rightAnswerCount,
         answerCount: context.answerCount,
         time: Date.now() - context.startDate,
         finalTime: context.finalTime,
         mapServerUrl: mapServerUrl,
         mapId: context.mapId,
         mapInfo: mapInfo
      };

      setTimeout(function() { socket.emit('displayPage', downData); }, debugDelay);
   }
   
   function getGameTexts(texts)
   {
      var t = 
      {
         questionnaire: texts.questionnaire,
         level: texts.level,
         score: texts.score,
         time: texts.time,
         stop: texts.stop,
         finish: texts.finish,
         areYouSureStopGame: texts.areYouSureStopGame,
         yes: texts.yes,
         no: texts.no,
         previous: texts.previous,
         submit: texts.submit,
         next: texts.next,
         question: texts.question
      };
      
      return t;
   }
   
   socket.on('levelChoice', function(upData)
   {
      var context;
      
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(fContext)
      { 
         context = fContext;
         upData.siteLanguageId = context.siteLanguageId;
         quizData.getLevelChoiceDownData(upData, emitUpdateSelects);
      });
      
      function emitUpdateSelects(downData)
      {
         context.questionnaireId = downData.questionnaireId;
         context.questionnaireLanguageId = downData.questionnaireLanguageId;
         context.levelId = downData.levelId;
         context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
         
         setTimeout(function() { socket.emit('updateSelects', downData); }, debugDelay);
      }
   });
   
   socket.on('languageChoice', function(upData)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      {          
         var siteLanguageId;
         languages.forEach(function(language)
         {
            if (upData.languageId == language.id) siteLanguageId = language.id;
         });

         if (siteLanguageId)
         {
            context.siteLanguageId = siteLanguageId;
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
         }
         
         var texts = translate(context.siteLanguageId).texts;
         
         if (upData.page == 'signUp')
         {            
            var downData = getSignUpTexts(texts);
            setTimeout(function() { socket.emit('updateSignUp', downData); }, debugDelay);
         }
         else
         {
            var downData = getIndexTexts(texts);
            setTimeout(function() { socket.emit('updateIndex', downData); }, debugDelay);
         }
      });
   });
   
   socket.on('scoreTab', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         context.scoreTab = data.n;
         context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
      });
   });
   
   socket.on('getTables', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         var tables = [];
         var i = 0;
         var n = dTable.length;
         dTable.forEach(function(dayCount)
         {
            userData.getScoreTable(data.questionnaireId, data.levelId, dayCount, 10, context.user, function(table)
            {
               table.d = dayCount;
               tables.push({ rows: table, d: dayCount });
               ++i;
               if (i == n) setTimeout(function() { socket.emit('tables',  { questionnaireId: data.questionnaireId, levelId: data.levelId, tables: tables }); }, debugDelay);
            });
         });
      });
   });
      
   socket.on('signUp', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      socket.request.session.userId = undefined;
      socket.request.session.save(function(err)
      {
         if (err) { console.log(err); /* Todo: Handle error */ } 
         getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
         { 
            context.tmpName = data.name;      
            emitDisplaySignUp(context);
         });
      });
   });
   
   socket.on('cancelSignUp', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         context.signUpMessages = { name1: false, name2: false, pass1a: false, pass1b: false, pass2: false, error: false };
         context.sendScoreWithSignUp = false;
         emitDisplayIndex(context);
      });
   });
   
   socket.on('submitSignUp', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      {
         context.tmpName = data.name;
         var ok = true;

         context.signUpMessages.name1 = (data.name.length < 2 || data.name.length > 16);
         if (context.signUpMessages.name1) ok = false;
         
         context.signUpMessages.name2 = false;
         
         context.signUpMessages.pass1a = (data.pass1.length < 8);
         if (context.signUpMessages.pass1a) ok = false;
         
         context.signUpMessages.pass1b = !(/^(?=.*[_,?;.:!$*+=&-])[A-Za-z0-9c_,?;.:!$*+=&-]+$/.test(data.pass1));
         if (context.signUpMessages.pass1b) ok = false;
         
         context.signUpMessages.pass2 = (data.pass1 !== data.pass2);
         if (context.signUpMessages.pass2) ok = false;

         if (ok)
         {
            userData.tryAddUser(data.name, data.pass1, context, function(err, id)
            {
               if (err)
               {
                  console.log('err: ' + err);
                  context.signUpMessages.error = true;
                  signUpFailed();
               }
               else if (!id)
               {
                  context.signUpMessages.name2 = true;
                  signUpFailed();
               }
               else
               {
                  socket.request.session.userId = id;
                  socket.request.session.save(function(err)
                  {
                     if (err) { console.log(err); /* Todo: Handle error */ } 
                     getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
                     {
                        if (context.sendScoreWithSignUp)
                        {
                           userData.addScore(context.questionnaireId, context.levelId, context.user, context.rightAnswerCount, context.finalTime);
                           context.sendScoreWithSignUp = false;
                        }
                        
                        emitDisplayIndex(context);
                     });
                  });
               }
            });       
         }
         else
         {
            signUpFailed();
         }
         
         function signUpFailed()
         {
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
         
            var downData = 
            {
               name1Message: context.signUpMessages.name1,
               name2Message: context.signUpMessages.name2,
               pass1aMessage: context.signUpMessages.pass1a,
               pass1bMessage: context.signUpMessages.pass1b,
               pass2Message: context.signUpMessages.pass2,
               errorMessage: context.signUpMessages.error
            };
            setTimeout(function() { socket.emit('signUpError', downData); }, debugDelay);
         }
      });
   });
   
   socket.on('signIn', function(data)
   {     
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context1)
      {
         userData.findUserId(data.name, data.pass, function(err, id)
         {
            if (err)
            {
               console.log(err);
               setTimeout(function() { socket.emit('indexError', {}); }, debugDelay);
            }
            else if (!id)
            {
               setTimeout(function() { socket.emit('unknownName', {}); }, debugDelay);
            }
            else
            {
               socket.request.session.userId = id;    
               socket.request.session.save(function(err)
               {
                  if (err) { console.log(err); /* Todo: Handle error */ } 

                  getContext(socket.request.session, socket.request.sessionID, cookies, function(context2)
                  {
                     if (context1.sendScoreWithSignUp)
                     {
                        context2.questionnaireId = context1.questionnaireId;
                        context2.levelId = context1.levelId;
                        userData.addScore(context1.questionnaireId, context1.levelId, context2.user, context1.rightAnswerCount, context1.finalTime);
                        context1.sendScoreWithSignUp = false;
                        context1.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
                     }

                     emitDisplayIndex(context2);
                  });
               });
            }
         });
      });
   });
   
   socket.on('signOut', function()
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context1)
      {
         var name;
         if (context1.user) name = context1.user.name;
         
         socket.request.session.userId = undefined;
         socket.request.session.save(function(err)
         {
            if (err) { console.log(err); /* Todo: Handle error */ }
            getContext(socket.request.session, socket.request.sessionID, cookies, function(context2)
            {
               if (name) context2.tmpName = name;
               emitDisplayIndex(context2);
            });
         });
      });
   });
   
   socket.on('newGame', function()
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      {
         quizData.getLevelChoiceDownData(context, function(downData)
         {
            quizData.getLevelMap(function(levelMap)
            {
               var levelId = levelMap[downData.questionnaireId][downData.levelId];

               childProcess.exec('./produce_questions.exe ' + levelId + ' ' + downData.questionnaireLanguageId + dbParameters, function(err, stdout, stderr)
               {
                  if (err)
                  {
                     console.log(stderr);
                     context.quizId = undefined;
                     context.displayedQuestion = undefined;
                     context.questions = undefined;
                     context.questionStates = [];
                     context.answerCount = undefined;
                     context.rightAnswerCount = undefined;
                     context.startDate = undefined;
                     context.finalTime = undefined;
                  }
                  else
                  {
                     context.quizId = shortId.generate();
                     context.displayedQuestion = 0;
                     context.questions = JSON.parse(stdout);
                     context.questionStates = [];
                     context.questionnaireName = downData.questionnaireName;
                     context.mapId = downData.mapId;
                     context.levelName = downData.levelName;
                     context.answerCount = 0;
                     context.rightAnswerCount = 0;
                     context.startDate = Date.now();
                     context.finalTime = undefined;

                     context.questions.forEach(function(question, iQuestion)
                     {
                        context.questionStates.push({ answered: false, choiceStates: [] });
                        question.choices.forEach(function(choice, iChoice)
                        {
                           // 0 <= choiceStates[iChoice] <= 3 :
                           // bit 0 = 1 if choice is checked.
                           // bit 1 = 1 if answer has been submitted and if the choice is right
                           if (question.isMultiple) context.questionStates[iQuestion].choiceStates.push(0);
                           else context.questionStates[iQuestion].choiceStates.push(iChoice == 0 ? 1 : 0);
                        });
                     });
                  }

                  emitDisplayGame(context);
               });
            });
         });
      });
   });
   
   socket.on('stopGame', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         context.quizId = undefined;
         context.displayedQuestion = undefined;
         context.questions = undefined;
         context.questionStates = [];
         
         if (context.user || !context.finalTime)
         {
            emitDisplayIndex(context);
         }
         else
         {
            context.sendScoreWithSignUp = true;
            context.saver.save(function(err) { emitDisplaySignUp(context); });
         }
      });
   });
   
   socket.on('submit', function(data)
   {
      var receptionDate = Date.now();
      
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         if (context.quizId && context.quizId == data.quizId)
         {
            var questionState = context.questionStates[data.question];

            if (!questionState.answered)
            {   
               ++context.answerCount;
               if (context.answerCount == context.questions.length)
               {
                  context.finalTime = (0.001 * (receptionDate - context.startDate)).toFixed(3);
               }
               
               questionState.answered = true;
               var question = context.questions[data.question];
               var error = false;
               
               data.checks.forEach(function(check, i)
               {
                  var right = question.choices[i].isRight;
                  // Note: With 'choiceStates[index] = value;' the value will not be saved to database by mongoose
                  // -> use 'choiceStates.set(index, value);' instead
                  questionState.choiceStates.set(i, (check ? 1 : 0) + (right ? 2 : 0));
                  if (Boolean(check) != Boolean(right)) error = true;
               });
               
               if (!error) ++context.rightAnswerCount;
               
               if (context.answerCount == context.questions.length && context.user)
               {
                  userData.addScore(context.questionnaireId, context.levelId, context.user, context.rightAnswerCount, context.finalTime);
               }

               context.saver.save(function(err) { if (err) { console.log(err); /* Todo: handle error */ } });
            }
            
            setTimeout(function() { socket.emit('updateQuestions', getOutData(context)); }, debugDelay);
         }
         else
         {
            context.quizId = undefined;
            context.displayedQuestion = undefined;
            context.questions = undefined;
            context.questionStates = [];
            emitDisplayIndex(context);
         }
      }); 
   });
   
   socket.on('changeQuestion', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         if (context.quizId && context.quizId == data.quizId)
         {
            context.displayedQuestion = data.displayedQuestion;
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } }); 
            socket.emit('updateQuestions', getOutData(context));
         }
         else
         {
            context.quizId = undefined;
            context.displayedQuestion = undefined;
            context.questions = undefined;
            context.questionStates = [];
            emitDisplayIndex(context);
         }
      });
   });
   
   socket.on('timeRequest', function()
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         setTimeout(function() { socket.emit('time', Date.now() - context.startDate); }, debugDelay);
      });
   });
         
   function extractCookies(cookieString)
   {
      if (!cookieString) return {};
      var cookieObject = {};
      var cookieArray = cookieString.split(';');

      cookieArray.forEach(function(cookie)
      {
         cookieParts = cookie.split('=');
         cookieObject[cookieParts[0].trim()] = cookieParts[1].trim();
      });

      return cookieObject;
   }
   
   function getOutData(context)
   {
      var outData =
      {
         quizId: context.quizId,
         rightAnswerCount: context.rightAnswerCount,
         answerCount: context.answerCount,
         questionStates: [],
         mapInfo: [],
         finalTime: context.finalTime
      };
      
      context.questionStates.forEach(function(questionState, i)
      {
         if (questionState.answered)
         {
            var outState = { index: i, choiceStates: [] };
            questionState.choiceStates.forEach(function(state, j)
            {
               var comment = context.questions[i].choices[j].comment;
               outState.choiceStates.push({ state: state, comment: comment.length ? comment : undefined } );
            });
            outData.questionStates.push(outState);
            outData.mapInfo.push(
            {
               mapIds: context.questions[i].mapIds,
               framingLevel: context.questions[i].framingLevel,
               mode: context.questions[i].mode,
               categories: context.questions[i].categories
            });
         }
      });
      
      return outData;
   }
});

function getContext(session0, sessionId, cookies, callback)
{
   if (session0.userId)
   {
      userData.getUser(session0.userId, function(err, user)
      {
         if (err)
         {
            console.log(err);
            noUser();
         }
         else if (!user)
         {
            console.log('!user');
            noUser();
         }
         else 
         {
            user.context.user = user;
            user.context.saver = user;
            callback(user.context);
         }
      });
   }
   else
   {
      noUser();
   }
   
   function noUser()
   {
      userData.getSession(sessionId, function(err, session)
      {
         if (err)
         {
             console.log(err);
            // Todo: Hanlde error
         }
         else if (!session)
         {
            // 'session' is undefined when the session has not yet been saved (case of the first request of the session). 
            // In this case, we save the session, then call again 'getContext'
            session0.save(function(err)
            {
               // Todo: Handle error
               if (err) console.log(err);
               getContext(session0, sessionId, cookies, callback);
            });
         }
         else 
         {
            if (session.context)
            {
               session.context.session = session;
               session.context.saver = session;
               callback(session.context);
            }
            else
            {
               session.context =
               {
                  siteLanguageId: cookies.siteLanguageId ? cookies.siteLanguageId : defaultLanguageId,
                  questionnaireId: cookies.questionnaireId,
                  questionnaireLanguageId: cookies.questionnaireLanguageId,
                  levelId: cookies.levelId,
                  currentPage: pages.index,
                  signUpMessages: { name1: false, name2: false, pass1a: false, pass1b: false, pass2: false, error: false },
                  tmpName: '',
                  scoreTab: 1,
                  sendScoreWithSignUp: false
               };
               
               session.context.session = session;
               session.context.saver = session;
               callback(session.context);
               
               session.save(function(err)
               {
                  if (err)
                  {
                     console.log(err);
                     // Todo: Handle error
                  }
               });
            }
         }
      });
   }
}

if (!config.port) throw new Error("No 'port' value in config.js");
console.log('Quiz server listening on port ' + config.port + '...');
server.listen(config.port);
