var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
//var session = require('express-session');
//var sharedsession = require("express-socket.io-session");
var cookieParser = require('cookie-parser');

app.use(express.static('public'));
app.use(morgan('dev'));

app.use(cookieParser('ke7Hèq*fG5ùZ'));
/*app.use(session(
{
  secret: 'ke7Hèq*fG5ùZ',
  resave: true,
  saveUninitialized: true
}));*/
//io.use(sharedsession(session)); 

app.get('/', function(req, res)
{
   /*console.log('req.session.country: ' + req.session.country);
   var selectedCountry = req.session.country;
   if (!selectedCountry)
   {
      selectedCountry = 'France';
      req.session.country = 'France';
   }*/
   
   console.log("req.cookies.country: " + req.cookies.country);
   var selectedCountry = req.cookies.country;
   if (!selectedCountry)
   {
      selectedCountry = "France";
      res.cookie('country', "France");
   }
   
   res.render('index.ejs', { countries: ['France', 'Allemagne', 'Italie'], selection: selectedCountry });
});

io.on('connection', function(socket)
{
   socket.on('countryChoice', function(country)
   {
      console.log(country);
      //console.log(socket.handshake);
      //console.log(socket.handshake.headers.cookie);
   });
});

console.log('Server listening on port 3000...');
server.listen(3000);
