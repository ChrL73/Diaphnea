var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var bodyparser = require("body-parser");
var session = require('express-session');

app.use(express.static('public'));
app.use(morgan('dev'));
app.use(bodyparser.urlencoded({ extended: false }));

var sessionMiddleware = session(
{
  secret: 'ke7Hèq*fG5ùZ',
  resave: false,
  saveUninitialized: true
});

io.use(function(socket, next)
{
   sessionMiddleware(socket.request, socket.request.res, next);
});

app.use(sessionMiddleware);

app.all('/', function(req, res)
{
   console.log('req.body.name: ' + req.body.name);
   req.session.name = req.body.name;   
   res.render('index.ejs', { name: req.body.name});
});

io.on('connection', function(socket)
{
   socket.on('click', function(data)
   {
      console.log(data);
      console.log("socket.request.session.name: " + socket.request.session.name);
   });
});

console.log('Server listening on port 3000...');
server.listen(3000);
