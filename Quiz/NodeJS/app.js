var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var bodyparser = require("body-parser");
var cookieParser = require('cookie-parser');
var session = require('express-session');
var favicon = require('serve-favicon');

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

/*userData.tryAddUser('Bob', 'blop', function(err, id)
{
   console.log('err: ' + err);
   console.log('id: ' + id);
});*/
//userData.displayAllUsers();
/*userData.findUserId('Bob', 'blop', function(err, userId)
{
   console.log('err: ' + err);
   console.log('userId: ' + userId);

   userData.getUser(userId, function(err, user)
   {
      console.log('err: ' + err);
      console.log('user: ' + user);
      
      userData.removeUser(userId, function(err, count)
      {
         console.log('err: ' + err);
         console.log('count: ' + count);
         
         userData.displayAllUsers();
      });
   });
});*/

app.all('/', function(req, res)
{
   var context = getContext(req.session, req.cookies);
   
   if (req.body.enterSignUp)
   {
      enterSignUp(req, res, context, 'false');
      return;
   }
   else if (req.body.submitSignUp)
   {
      if (!submitSignUp(req, res, context)) return;
   }
   
   quizData.getLevelChoiceDownData(context, renderView);
   
   function renderView(downData)
   {
      if (context.user) downData.userName = context.user.name;
      
      downData.siteLanguageList = languages;
      downData.siteLanguageId = context.siteLanguageId;
      downData.texts = translate(context.siteLanguageId).texts;
      
      res.render('index.ejs', { data: downData });
   }
});

function getContext(session, cookies)
{
   var context = {};
   
   if (session.userId) context.user = userData.getUser(session.userId);
   if (context.user)
   {
      context.siteLanguageId = user.pageParameters.siteLanguageId,
      context.questionnaireId = user.pageParameters.questionnaireId,
      context.languageId = user.pageParameters.languageId,
      context.levelId = user.pageParameters.levelId     
   }
   
   if (!context.siteLanguageId) context.siteLanguageId = session.siteLanguageId;
   if (!context.siteLanguageId) context.siteLanguageId = cookies.siteLanguageId;
   if (!context.siteLanguageId) context.siteLanguageId = defaultLanguageId;
   
   if (!context.questionnaireId) context.questionnaireId = session.questionnaireId;
   if (!context.questionnaireId) context.questionnaireId = cookies.questionnaireId;
   
   if (!context.languageId) context.languageId = session.languageId;
   if (!context.languageId) context.languageId = cookies.languageId;
   
   if (!context.levelId) context.levelId = session.levelId;
   if (!context.levelId) context.levelId = cookies.levelId;
   
   return context;
}

function enterSignUp(req, res, context, reload)
{
   var data =
   {
      name: req.body.name,
      pass1: reload ? req.body.pass1 : '',
      pass2: reload ? req.body.pass2 : '',
      siteLanguageList: languages,
      siteLanguageId: context.siteLanguageId,
      texts: translate(context.siteLanguageId).texts,
      reload: reload
   };
   
   res.render('sign_up.ejs', { data: data });
}

function submitSignUp(req, res, context)
{
   if (req.body.name.length < 2 || req.body.name.length > 16 || req.body.pass1.length < 8
      || !(/^(?=.*[_,?;.:!$*+=&-])[A-Za-z0-9c_,?;.:!$*+=&-]+$/.test(req.body.pass1)) || req.body.pass1 !== req.body.pass2)
   {
      enterSignUp(req, res, context, 'true');
      return false;
   }
   
   return true;
}

app.use(function(req, res)
{
   res.redirect('/');
});

io.on('connection', function(socket)
{
   socket.on('levelChoice', function(upData)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);  
      var context = getContext(socket.request.session, cookies);
      upData.siteLanguageId = context.siteLanguageId;
      
      quizData.getLevelChoiceDownData(upData, emitUpdateSelects);
   });
   
   function emitUpdateSelects(downData)
   {
      socket.emit('updateSelects', downData);
   }

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
