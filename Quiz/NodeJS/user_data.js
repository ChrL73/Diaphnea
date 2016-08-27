var mongoose = require('mongoose');
var crypto = require('crypto');

var contextSchema = mongoose.Schema(
{
   siteLanguageId: String,
   questionnaireId: String,
   questionnaireLanguageId: String,
   levelId: String,
   currentPage: Number,
   questions: mongoose.Schema.Types.Mixed,
   displayedQuestion: Number,
   questionStates: [{ answered: Boolean, choiceStates: [Number] }]
});

var sessionSchema = mongoose.Schema(
{
   _id: String,
   session: mongoose.Schema.Types.Mixed,
   expires: mongoose.Schema.Types.Mixed,
   context: contextSchema
});
var SessionModel = mongoose.model('Session', sessionSchema);

var userSchema = mongoose.Schema(
{
   name: String,
   sha1pass: String,
   context: contextSchema
});
var UserModel = mongoose.model('User', userSchema);

function getUser(id, callback)
{
   UserModel.findOne({ _id: id }, callback);
}

function getSession(id, callback)
{
   SessionModel.findOne({ _id: id }, callback);
}

function setContext(user, sessionID, gameState, callback)
{
   
}

module.exports.getUser = getUser;
module.exports.getSession = getSession;

