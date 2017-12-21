var config = {};

config.port = 3002;

// Replace this value with a value that is not in source control
config.sessionSecret = 'dev';

// If 'config.mapServerAddress' is not defined, the quiz server assumes that the map server runs on the same machine as the quiz server 
//config.mapServerAddress = '192.168.50.50';
config.mapServerPort = '3001';

//config.debugDelay = 2000;

module.exports = config;
