var express = require('express');
var app = express();
var server = require('http').Server(app);

app.use(express.static('public'));

app.get("/", function(req, res)
{
   res.render("index.ejs");
});

console.log("Quiz server listening on port 3000...");
server.listen(3000);