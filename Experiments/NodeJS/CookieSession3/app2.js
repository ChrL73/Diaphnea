var express = require('express');
var app = express();
var server = require('http').Server(app);
var favicon = require('serve-favicon');
var session = require('express-session');
var mongoStore = require('connect-mongo')(session);

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');

app.use(favicon('favicon.ico'));

app.use(session({
   secret: 'abc',
   resave: false,
   saveUninitialized: true,
   store: new mongoStore(
   {
      mongooseConnection: mongoose.connection,
      ttl: 60
   })
}))

app.use(function(req, res)
{
   res.render('app2.ejs', { sessionId: req.sessionID });
});

server.listen(3004);
console.log('Server listening on port 3004');
