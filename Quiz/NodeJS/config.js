var config = {};

config.port = process.env.PORT || 3002;

config.dbUrl = process.env.MONGODB_URI || 'mongodb://localhost/diaphnea';

config.sessionSecret = process.env.SESSION_SECRET || 'dev';

// If 'config.mapServerAddress' is not defined, the quiz server assumes that the map server runs on the same machine as the quiz server 
config.mapServerAddress = process.env.MAP_SERVER_ADDRESS;
config.mapServerPort = process.env.MAP_SERVER_PORT || '3001';
config.mapServerProtocol = process.env.PROTOCOL || 'http';
config.forceHttpsRedirection = (process.env.PROTOCOL === 'https' && process.env.HTTPS_URL);
config.httpsAddress = process.env.HTTPS_URL;

//config.debugDelay = 2000;

//config.frontEndIndex = 'quiz.ejs'; // Old front-end, removed from source control
config.frontEndIndex = 'reactIndex.ejs'; // React front-end

config.version = 'Alpha1.0';
config.sourceUrl = 'https://github.com/ChrL73/Diaphnea';
config.issueUrl = 'https://github.com/ChrL73/Diaphnea/issues';

module.exports = config;
