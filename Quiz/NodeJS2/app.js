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
            if (!locaIp && iface.family === 'IPv4' && iface.internal === false) 
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
         start: texts.start
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
         pass1aMessage: context.signUpMessages.pass1a,
         pass1bMessage: context.signUpMessages.pass1b,
         pass2Message: context.signUpMessages.pass2,
         errorMessage: context.signUpMessages.error
      };

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
         nameMustBeBetween2And16Chars: texts.nameMustBeBetween2And16Chars,
         passwordMustContainAtLeast8Chars: texts.passwordMustContainAtLeast8Chars,
         passwordMustContainOnlyLettersNumbersEtc: texts.passwordMustContainOnlyLettersNumbersEtc,
         twoPasswordsAreNotIdentical: texts.twoPasswordsAreNotIdentical,
         theNameIsAlreadyUsed: texts.theNameIsAlreadyUsed,
         internalServerError: texts.internalServerError
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
      
   socket.on('signUp', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      
      if (socket.request.session.userId) // Todo: Test this case
      {
         socket.request.session.userId = undefined;
         socket.request.session.save(function(err)
         {
            if (err) { console.log(err); /* Todo: Handle error */ } 
            continueSignUp();
         });
      }
      else
      {
         continueSignUp();
      }
      
      function continueSignUp()
      {
         getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
         { 
            context.tmpName = data.name;      
            emitDisplaySignUp(context);
         });
      }
   });
   
   socket.on('index', function(data)
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
      { 
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

               var cookies = extractCookies(socket.handshake.headers.cookie);
               getContext(socket.request.session, socket.request.sessionID, cookies, function(context)
               {
                  emitDisplayIndex(context);
               });
            });
         }
      });
   });
   
   socket.on('signOut', function()
   {
      var cookies = extractCookies(socket.handshake.headers.cookie);
      getContext(socket.request.session, socket.request.sessionID, cookies, function(context1)
      {
         var name = context1.user.name;
         
         socket.request.session.userId = undefined;    
         socket.request.session.save(function(err)
         {
            if (err) { console.log(err); /* Todo: Handle error */ } 
            getContext(socket.request.session, socket.request.sessionID, cookies, function(context2)
            {
               context2.tmpName = name;
               emitDisplayIndex(context2);
            });
         });
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
