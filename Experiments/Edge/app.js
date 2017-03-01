var server = require('http').createServer(function(req, res) { res.end(req.method); });
server.listen(3000);