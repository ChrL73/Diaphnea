var crypto = require('crypto');

function getRandomInt(valueCount)
{
   var buf = crypto.randomBytes(32);
   var i = buf.readUInt32LE(0);
   return i % valueCount;
}

module.exports.getRandomInt = getRandomInt;
