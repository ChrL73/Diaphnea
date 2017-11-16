var config = {};

config.port = 3000;

// Replace these values with values that are not in source control
config.sessionSecret = 'dev1';
config.cookieSecret = 'dev2';

// [todo] If 'config.mapServerAddress' is not define, we assume that the map server run on the same machine as the quiz server 
config.mapServerAddress = '192.168.50.129';
config.mapServerPort = '3001';

//config.debugDelay = 2000;

module.exports = config;
