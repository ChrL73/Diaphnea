var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var bodyparser = require("body-parser");
var cookieParser = require('cookie-parser');
var session = require('express-session');
var mongoStore = require('connect-mongo')(session);
var favicon = require('serve-favicon');
var childProcess = require('child_process');
var shortId = require('shortid');

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');
var quizData = require('./quiz_data');
var userData = require('./user_data');

var config = require('./config');
var translate = require('./translate');
var languages = translate().languages;
var defaultLanguageId = languages[0].id;

app.use(express.static('public'));
app.use(morgan('dev'));
app.use(bodyparser.urlencoded({ extended: false }));
app.use(favicon('public/favicon.ico'));

if (!config.cookieSecret) throw new Error("No 'cookieSecret' value in config.js");
app.use(cookieParser(config.cookieSecret));

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

var pages =
{
   index: 0,
   signUp: 1,
   game: 2,
};

var debugDelay = config.debugDelay ? config.debugDelay : 0;

quizData.getLevelMap(function(levelMap) { /*console.log(levelMap);*/ } );

app.all('/', function(req, res)
{
   getContext(req.session, req.sessionID, req.cookies, function(context)
   {
      if (req.body.siteLanguageSelect)
      {
         var siteLanguageId;
         languages.forEach(function(language)
         {
            if (req.body.siteLanguageSelect == language.id) siteLanguageId = language.id;
         });

         if (siteLanguageId) context.siteLanguageId = siteLanguageId;
      }

      var pageInBody = req.body.enterSignUp || req.body.submitSignUp || req.body.cancelSignUp ||
                       req.body.signIn || req.body.signOut || req.body.start || req.body.stop;

      if (req.body.enterSignUp || (!pageInBody && context.currentPage == pages.signUp))
      {
         enterSignUp(req, res, context, { reload: 'false', userExists: 'false', error: 'false' });
      }
      else if (req.body.submitSignUp)
      {
         submitSignUp(req, res, context);
      }
      else if (req.body.signIn)
      {
         signIn(req, res, context);
      }
      else if (req.body.signOut)
      {
         signOut(req, res, context);
      }
      else if (req.body.start || (!pageInBody && context.currentPage == pages.game))
      {
         game(req, res, context);
      }
      else if (req.body.stop)
      {
         stop(req, res, context);
      }
      else
      {
         index(req, res, context, { unknwon: 'false', error: 'false' });
      }
   });
});

app.use(function(req, res)
{
   res.redirect('/');
});
        
function index(req, res, context, flags)
{
   context.currentPage = pages.index;
   quizData.getLevelChoiceDownData(context, renderView);
   
   function renderView(downData)
   {
      if (context.user) downData.userName = context.user.name;
      
      context.questionnaireId = downData.questionnaireId;
      context.questionnaireLanguageId = downData.questionnaireLanguageId;
      context.levelId = downData.levelId;
      context.saver.save(function(err)
      {
         if (err) console.log(err);
         // Todo: handle error
      });
         
      downData.siteLanguageList = languages;
      downData.siteLanguageId = context.siteLanguageId;
      downData.texts = translate(context.siteLanguageId).texts;
      downData.flags = flags;
      
      if (flags.error == 'true') res.status(500);
      res.render('index.ejs', { data: downData });
   }
}

function game(req, res, context)
{
   var newGame = (context.currentPage != pages.game || !context.questions);
   context.currentPage = pages.game;
   
   if (newGame)
   {
      quizData.getLevelChoiceDownData(context, function(downData)
      {
         quizData.getLevelMap(function(levelMap)
         {
            var levelId = levelMap[downData.questionnaireId][downData.levelId];

            childProcess.exec('./produce_questions.exe ' + levelId + ' ' + downData.questionnaireLanguageId, function(err, stdout, stderr)
            {
               if (err)
               {
                  console.log(stderr);
                  context.quizId = undefined;
                  context.displayedQuestion = undefined;
                  context.questions = undefined;
                  context.questionStates = [];
               }
               else
               {
                  context.quizId =  shortId.generate();
                  context.displayedQuestion = 0;
                  context.questions = JSON.parse(stdout);
                  context.questionStates = [];
                  
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

               renderGameView();
            });
         });
      });
   }
   else
   {
      renderGameView();
   }
   
   function renderGameView()
   {
      context.saver.save(function(err)
      {
         if (err) console.log(err);
         // Todo: handle error
      });
      
      var data =
      {
         texts: translate(context.siteLanguageId).texts,
         quizId: context.quizId,
         displayedQuestion: context.displayedQuestion,
         questions: context.questions,
         questionStates: context.questionStates,
         error: !context.quizId // Todo: handle error in view
      };
      
      res.render('game.ejs', { data: data });
   }
}

function stop(req, res, context)
{
   context.quizId = undefined;
   context.displayedQuestion = undefined;
   context.questions = undefined;
   context.questionStates = [];
   index(req, res, context, { unknwon: 'false', error: 'false' });
}

function enterSignUp(req, res, context0, flags)
{
   if (req.session.userId) // Todo: test this case
   {
      req.session.userId = undefined;
      getContext(req.session, req.sessionID, req.cookies, function(context)
      {
         continueSignUp(context);
      });
   }
   else
   {
      continueSignUp(context0);
   }
   
   function continueSignUp(context)
   {
      context.currentPage = pages.signUp;
      context.saver.save(function(err)
      {
         if (err) console.log(err);
         /* Todo: handle error */
      });

      var data =
      {
         name: req.body.name,
         pass1: req.body.pass1 ? req.body.pass1 : '',
         pass2: req.body.pass2 ? req.body.pass2 : '',
         siteLanguageList: languages,
         siteLanguageId: context.siteLanguageId,
         texts: translate(context.siteLanguageId).texts,
         flags: flags
      };

      if (flags.error == 'true') res.status(500);
      res.render('sign_up.ejs', { data: data });   
   }
}

function signIn(req, res, context)
{
   userData.findUserId(req.body.name, req.body.pass, function(err, id)
   {
      if (err)
      {
         console.log(err);
         index(req, res, context, { unknwon: 'false', error: 'true' });
      }
      else if (!id)
      {
         index(req, res, context, { unknwon: 'true', error: 'false' });
      }
      else
      {
         req.session.userId = id;
         res.redirect('/');
      }
   });
}

function submitSignUp(req, res, context)
{
   if (req.body.name.length < 2 || req.body.name.length > 16 || req.body.pass1.length < 8
      || !(/^(?=.*[_,?;.:!$*+=&-])[A-Za-z0-9c_,?;.:!$*+=&-]+$/.test(req.body.pass1)) || req.body.pass1 !== req.body.pass2)
   {
      enterSignUp(req, res, context, { reload: 'true', userExists: 'false', error: 'false' });
      return;
   }
   
   userData.tryAddUser(req.body.name, req.body.pass1, context, function(err, id)
   {
      if (err)
      {
         console.log('err: ' + err);
         enterSignUp(req, res, context, { reload: 'true', userExists: 'false', error: 'true' });
      }
      else if (!id)
      {
         enterSignUp(req, res, context, { reload: 'true', userExists: 'true', error: 'false' });
      }
      else
      {
         req.session.userId = id;       
         getContext(req.session, req.sessionID, req.cookies, function(context)
         {
            index(req, res, context, { unknwon: 'false', error: 'false' });
         });
      }
   });
}

function signOut(req, res, context)
{
   req.session.userId = undefined;
   getContext(req.session, req.sessionID, req.cookies, function(context)
   {
      index(req, res, context, { unknwon: 'false', error: 'false' }); 
   });
}

function getContext(session0, sessionId, cookies, callback)
{
   if (session0.userId)
   {
      userData.getUser(session0.userId, function(err, user)
      {
         if (err)
         {
             console.log(err);
            // Todo: Hanlde error
         }
         else if (!user)
         {
            console.log('!user');
            // Todo: Hanlde error
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
               // Todo: handle error
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
                  currentPage: pages.index
               };
               
               session.context.session = session;
               session.context.saver = session;
               callback(session.context);
               
               session.save(function(err)
               {
                  if (err)
                  {
                     console.log(err);
                     // Todo: handle error
                  }
               });
            }
         }
      });
   }
}

io.on('connection', function(socket)
{
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
         context.saver.save(function(err) { if (err) { console.log(err); /* Todo: handle error */ } });
         setTimeout(function() { socket.emit('updateSelects', downData); }, debugDelay);
      }
   });
   
   socket.on('changeQuestion', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         if (context.quizId && context.quizId == data.quizId)
         {
            context.displayedQuestion = data.displayedQuestion;
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: handle error */ } }); 
            socket.emit('updateQuestions', getOutData(context));
         }
         else
         {
            setTimeout(function() { socket.emit('reload'); }, debugDelay);
         }
      });
   });
   
   socket.on('submit', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         if (context.quizId && context.quizId == data.quizId)
         {
            var questionState = context.questionStates[data.question];
            var question = context.questions[data.question];

            if (!questionState.answered)
            {  
               questionState.answered = true;
               data.checks.forEach(function(check, i)
               {
                  // Note: With 'choiceStates[index] = value;' the value will not be saved to database by mongoose
                  // -> use 'choiceStates.set(index, value);' instead
                  questionState.choiceStates.set(i, (check ? 1 : 0) + (question.choices[i].isRight ? 2 : 0));
               });

               context.saver.save(function(err) { if (err) { console.log(err); /* Todo: handle error */ } });
            }
            
            setTimeout(function() { socket.emit('updateQuestions', getOutData(context)); }, debugDelay);
         }
         else
         {
            setTimeout(function() { socket.emit('reload'); }, debugDelay);
         }
      }); 
   });
   
   function extractCookies(cookieString)
   {
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
      var outData = { quizId: context.quizId, questionStates: [] };
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
         }
      });
      
      return outData;
   }
});

if (!config.port) throw new Error("No 'port' value in config.js");
console.log('Quiz server listening on port ' + config.port + '...');
server.listen(config.port);
