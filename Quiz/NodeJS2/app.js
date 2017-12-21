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

app.all('/', function(req, res, next)
{
   res.sendFile(__dirname + '/quiz.html');
});

app.use(function(req, res)
{
   res.redirect('/');
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
         
      }
      else
      {
         quizData.getLevelChoiceDownData(context, function(downData)
         {
            context.questionnaireId = downData.questionnaireId;
            context.questionnaireLanguageId = downData.questionnaireLanguageId;
            context.levelId = downData.levelId;
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });      
            emitDisplayIndex(downData, context);
         });
      }
   });
         
   function emitDisplayIndex(downData, context)
   {
      var texts = translate(context.siteLanguageId).texts;

      downData.texts =
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
         start: texts.start
      }      

      downData.page = 'index';
      if (context.user) downData.userName = context.user.name;  
      downData.siteLanguageList = languages;
      downData.siteLanguageId = context.siteLanguageId;
      downData.unknown = context.indexMessages.unknown;
      downData.error = context.indexMessages.error;  

      setTimeout(function() { socket.emit('displayPage', downData); }, debugDelay);
   }
   
   function emitDisplaySignUp(context)
   {
      var texts = translate(context.siteLanguageId).texts;
         
      var downData =
      {
         page: 'signUp',
         texts:
         {
            name: texts.name,
            password: texts.password,
            confirmPassword: texts.confirmPassword,
            signUp: texts.signUp,
            cancel: texts.cancel
         },
         name: context.tmpName,
         siteLanguageList: languages,
         siteLanguageId: context.siteLanguageId
      };

      setTimeout(function() { socket.emit('displayPage', downData); }, debugDelay);
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

         if (siteLanguageId) context.siteLanguageId = siteLanguageId;
         
         upData.questionnaireId = context.questionnaireId;
         upData.siteLanguageId = context.siteLanguageId;
         upData.levelId = context.levelId;
         
         quizData.getLevelChoiceDownData(upData, function(downData)
         {
            context.questionnaireId = downData.questionnaireId;
            context.questionnaireLanguageId = downData.questionnaireLanguageId;
            context.levelId = downData.levelId;
            context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });      
            emitDisplayIndex(downData, context);
         });
      });
   });
      
   socket.on('signUp', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie); 
      if (socket.request.session.userId) socket.request.session.userId = undefined; // Todo: Test this case
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
         context.currentPage = pages.signUp;
         context.tmpName = data.name;
         context.saver.save(function(err) { if (err) { console.log(err); /* Todo: Handle error */ } });
         
         emitDisplaySignUp(context);
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
                  currentPage: pages.index,
                  indexMessages: { unknown: false, error: false },
                  tmpName: ''
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
