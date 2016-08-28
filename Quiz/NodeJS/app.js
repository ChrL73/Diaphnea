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
   store: new mongoStore({ mongooseConnection: mongoose.connection })
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
      /*else if (req.body.submitSignUp)
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
      }*/
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
      context.saver.save(function(err) { /* Todo: handle error */ });
         
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
   
}

function enterSignUp(req, res, context, flags)
{
   
}

function signIn(req, res, context)
{
   
}

function submitSignUp(req, res, context)
{
   
}

function signOut(req, res, context)
{
   
}

function getContext(session0, sessionId, cookies, callback)
{
   if (session0.userId)
   {
      userData.getUser(session0.userId, function(err, user)
      {
         
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
         context.saver.save(function(err) { /* Todo: handle error */ });
         socket.emit('updateSelects', downData);
      }
   });
   
   socket.on('changeQuestion', function(displayedQuestion)
   {
      
   });
   
   socket.on('submit', function(data)
   {
      
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
});

if (!config.port) throw new Error("No 'port' value in config.js");
console.log('Quiz server listening on port ' + config.port + '...');
server.listen(config.port);
