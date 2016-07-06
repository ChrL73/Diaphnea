var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var cookieParser = require('cookie-parser');
var session = require('express-session');

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');
var quizData = require('./quiz_data');
var userData = require('./user_data');

var config = require('./config');
var translate = require('./translate');

app.use(express.static('public'));
app.use(morgan('dev'));

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
   var user;
   if (req.session.userId) user = userData.getUser(req.session.userId);
   
   var upData;
   if (user)
   {
      upData =
      {
         questionnaireId: user.preferences.questionnaireId,
         languageId: user.preferences.languageId,
         levelId: user.preferences.levelId
      };
   }
   else
   {
      upData =
      {
         questionnaireId: req.cookies.questionnaireId,
         languageId: req.cookies.languageId,
         levelId: req.cookies.levelId
      };
   }
   
   quizData.getLevelChoiceDownData(upData, renderView);
   
   function renderView(downData)
   {
      downData.siteLanguageList = translate().languages;
      
      if (user)
      {
         downData.userName = user.name;
         downData.siteLanguageId = user.preferences.siteLanguageId;
      }
      else
      {   
         downData.siteLanguageId = req.cookies.siteLanguageId;
      }
      
      if (!downData.siteLanguageId) downData.siteLanguageId = translate().languages[0].id;
      console.log(downData.siteLanguageId);
      downData.texts = translate(downData.siteLanguageId).texts;
      
      res.render('index.ejs', { data: downData });
   }
});

app.use(function(req, res)
{
   res.redirect('/');
});

io.on('connection', function(socket)
{
   socket.on('levelChoice', function(upData)
   {
      quizData.getLevelChoiceDownData(upData, emitUpdateSelects);
   });
   
   function emitUpdateSelects(downData)
   {
      socket.emit('updateSelects', downData);
   }
});

if (!config.port) throw new Error("No 'port' value in config.js");
console.log('Quiz server listening on port ' + config.port + '...');
server.listen(config.port);
