var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var bodyparser = require("body-parser");
var cookieParser = require('cookie-parser');
var session = require('express-session');
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
  saveUninitialized: true
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

app.all('/', function(req, res)
{
   updateContext(req.session, req.cookies, function(context)
   {
      if (context.user && req.body.siteLanguageSelect)
      {
         var siteLanguageId;
         languages.forEach(function(language)
         {
            if (req.body.siteLanguageSelect == language.id) siteLanguageId = language.id;
         });

         if (siteLanguageId)
         {
            context.siteLanguageId = siteLanguageId;
            var parameters = { siteLanguageId: siteLanguageId };
            userData.updateParameters(context.user, parameters, function(err)
            {
               // Todo: handle error
            });
         }
      }

      var pageInBody = req.body.enterSignUp || req.body.submitSignUp || req.body.cancelSignUp ||
                       req.body.signIn || req.body.signOut || req.body.start || req.body.stop;

      if (req.body.enterSignUp || (!pageInBody && req.session.currentPage == pages.signUp))
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
      else if (req.body.start || (!pageInBody && req.session.currentPage == pages.game))
      {
         game(req, res, context);
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
   req.session.currentPage = pages.index;
   quizData.getLevelChoiceDownData(context, renderView);
   
   function renderView(downData)
   {
      if (context.user)
      {
         downData.userName = context.user.name;
         var parameters = { questionnaireId: downData.questionnaireId, languageId: downData.languageId, levelId: downData.levelId };
         userData.updateParameters(context.user, parameters, function(err) { /* Todo: handle error */ });
      }
      
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
   req.session.currentPage = pages.game;
   
   if (!context.questionnaireId || !context.languageId || !context.languageId)
   {
      quizData.getLevelChoiceDownData(context, continuation);
   }
   else
   {
      continuation();
   }
   
   function continuation(downData)
   {
      var qData = {};
      if (downData)
      {
         qData.questionnaireId = downData.questionnaireId;
         qData.languageId = downData.languageId;
         qData.levelId = downData.levelId;
      }
      else
      {
         qData.questionnaireId = context.questionnaireId;
         qData.languageId = context.languageId;
         qData.levelId = context.levelId;
      }
      
      var questionProducer = childProcess.spawn('node', ['produceQuestions.js']);
      questionProducer.stdin.write(JSON.stringify(qData));
      questionProducer.stdout.on('data', function(jsonData)
      {
         var data = JSON.parse(jsonData);
         console.log(data);
      });
      questionProducer.stderr.on('data', function(data) { console.log(data.toString()); });

      var data = { texts: translate(context.siteLanguageId).texts };

      res.render('game.ejs', { data: data });
   }
}

function enterSignUp(req, res, context, flags)
{
   req.session.currentPage = pages.signUp;
   var data =
   {
      name: req.body.name,
      pass1: flags.reload ? req.body.pass1 : '',
      pass2: flags.reload ? req.body.pass2 : '',
      siteLanguageList: languages,
      siteLanguageId: context.siteLanguageId,
      texts: translate(context.siteLanguageId).texts,
      flags: flags
   };
   
   if (flags.error == 'true') res.status(500);
   res.render('sign_up.ejs', { data: data });
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
         updateContext(req.session, req.cookies, function(context)
         {
            index(req, res, context, { unknwon: 'false', error: 'false' });          
         });
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
   
   userData.tryAddUser(req.body.name, req.body.pass1, function(err, id)
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
         updateContext(req.session, req.cookies, function(context)
         {
            userData.updateParameters(context.user, context, function(err)
            {
               // Todo: handle error
               index(req, res, context, { unknwon: 'false', error: 'false' });
            });
         });
      }
   });
}

function signOut(req, res, context)
{
   req.session.userId = undefined;
   updateContext(req.session, req.cookies, function(context)
   {
      index(req, res, context, { unknwon: 'false', error: 'false' });          
   });
}

function updateContext(session, cookies, callback)
{
   var context = {};
   
   if (session.userId)
   {
      userData.getUser(session.userId, function(err, user)
      {
         // Todo: Handle error
         if (!err && user)
         {
            context.user = user;
            if (user.parameters)
            {
               context.siteLanguageId = user.parameters.siteLanguageId;
               context.questionnaireId = user.parameters.questionnaireId;
               context.languageId = user.parameters.languageId;
               context.levelId = user.parameters.levelId;
            }
         }
         
         continuation();
      });
   }
   else
   {
      continuation();
   }
                       
   function continuation()
   {
      if (!context.siteLanguageId) context.siteLanguageId = cookies.siteLanguageId;
      if (!context.siteLanguageId) context.siteLanguageId = defaultLanguageId;
      if (!context.questionnaireId) context.questionnaireId = cookies.questionnaireId
      if (!context.languageId) context.languageId = cookies.languageId;
      if (!context.levelId) context.levelId = cookies.levelId;

      callback(context);
   }
}

io.on('connection', function(socket)
{
   socket.on('levelChoice', function(upData)
   {
      var context;
      
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      updateContext(socket.request.session, cookies, function(fContext)
      { 
         context = fContext;
         upData.siteLanguageId = context.siteLanguageId;
         quizData.getLevelChoiceDownData(upData, emitUpdateSelects);
      });
      
      function emitUpdateSelects(downData)
      {
         if (context.user)
         {
            var parameters = { questionnaireId: downData.questionnaireId, languageId: downData.languageId, levelId: downData.levelId };
            userData.updateParameters(context.user, parameters, function(err) { /* Todo: handle error */ });
         }

         socket.emit('updateSelects', downData);
      }
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
