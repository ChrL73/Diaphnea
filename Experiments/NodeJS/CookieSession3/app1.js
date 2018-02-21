var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var session = require('express-session');
var mongoStore = require('connect-mongo')(session);
var favicon = require('serve-favicon');

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');

app.use(favicon('favicon.ico'));

var sessionMiddleware = session(
{
   secret: 'abc',
   resave: false,
   saveUninitialized: true,
   store: new mongoStore(
   {
      mongooseConnection: mongoose.connection,
      ttl: 60
   })
});
io.use(function(socket, next)
{
   sessionMiddleware(socket.request, socket.request.res, next);
});
app.use(sessionMiddleware);

app.use(function(req, res)
{
   res.render('app1.ejs', { sessionId: req.sessionID });
});

io.on('connection', function(socket)
{ 
   socket.request.session.save();
   socket.emit('message', 'Message from socket.io: Your session id is ' + socket.request.sessionID
               + '. Your cookie is ' + socket.handshake.headers.cookie);
});

server.listen(3003);
console.log('Server listening on port 3003');
