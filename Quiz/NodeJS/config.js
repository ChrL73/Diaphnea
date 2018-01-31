var config = {};

config.port = 3002;

// Replace this value with a value that is not in source control
config.sessionSecret = 'dev';

// If 'config.mapServerAddress' is not defined, the quiz server assumes that the map server runs on the same machine as the quiz server 
//config.mapServerAddress = '192.168.50.50';
config.mapServerPort = '3001';

//config.debugDelay = 2000;

//config.jQueryUrl = 'https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js';
config.jQueryUrl = 'jquery.min.js';

//config.bootstrapCssUrl = 'https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css';
config.bootstrapCssUrl = 'bootstrap.min.css';

//config.bootstrapJsUrl = 'https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js';
config.bootstrapJsUrl = 'bootstrap.min.js';

config.version = 'Alpha1.0';
config.sourceUrl = 'https://github.com/ChrL73/Diaphnea';
config.issueUrl = 'https://github.com/ChrL73/Diaphnea/issues';

module.exports = config;
