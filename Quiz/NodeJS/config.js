var config = {};

config.port = 3002;

// Replace this value with a value that is not in source control
config.sessionSecret = 'dev';

// If 'config.mapServerAddress' is not defined, the quiz server assumes that the map server runs on the same machine as the quiz server 
//config.mapServerAddress = '192.168.1.5';
config.mapServerPort = '3001';

//config.debugDelay = 2000;

//config.frontEndIndex = 'quiz.ejs'; // Old front-end, removed from source control
config.frontEndIndex = 'reactIndex.ejs'; // React front-end

config.version = 'Alpha1.0';
config.sourceUrl = 'https://github.com/ChrL73/Diaphnea';
config.issueUrl = 'https://github.com/ChrL73/Diaphnea/issues';

module.exports = config;
