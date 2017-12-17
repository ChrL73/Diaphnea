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

if (!config.mapServerPort) throw new Error("No 'mapServerPort' value in config.js");
var mapServerUrl;
if (config.mapServerAddress)
{
   mapServerUrl = 'http://' + config.mapServerAddress + ':' + config.mapServerPort;
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
            if (!locaIp && 'IPv4' === iface.family && iface.internal === false) 
            {
               locaIp = iface.address;
            }
         });
      }
   });
   mapServerUrl = 'http://' + locaIp + ':' + config.mapServerPort;
}

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
      res.render('quiz.ejs', {});
   });
});

app.use(function(req, res)
{
   res.redirect('/');
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
