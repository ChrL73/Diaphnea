var mongoose = require('mongoose');
var crypto = require('crypto');

var stateSchema = mongoose.Schema({ answered: Boolean, choiceStates: [Number] }, { _id: false });

var contextSchema = mongoose.Schema(
{
   siteLanguageId: String,
   questionnaireId: String,
   questionnaireLanguageId: String,
   levelId: String,
   questionnaireName: String,
   levelName: String,
   mapId: String,
   currentPage: Number,
   signUpMessages: { name1: Boolean, name2: Boolean, pass1a: Boolean, pass1b: Boolean, pass2: Boolean, error: Boolean },
   tmpName: String,
   quizId: String,
   questions: mongoose.Schema.Types.Mixed,
   displayedQuestion: Number,
   questionStates: [stateSchema],
   rightAnswerCount: Number,
   answerCount: Number,
   startDate: Date,
   finalTime: Number
}, { _id: false });

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

function tryAddUser(name, pass, context, callback)
{
   UserModel.findOne({ name: name }, findEnd);
   
   var newUser; 
   function findEnd(err, user)
   {
      if (err)
      {
         callback(err, null);
         return;
      }
      if (user)
      {
         // User name already exists
         callback(null, null);
         return;
      }
      
      var hash = crypto.createHash('sha1');
      hash.update(pass);
      var sha1 = hash.digest('hex');
      
      newUser = new UserModel();
      newUser.name = name;
      newUser.sha1pass = sha1;
      newUser.context = context;
      newUser.save(saveEnd);
   }
         
   function saveEnd(err)
   {
      if (err)
      {
         callback(err, null);
         return;
      }
      
      callback(null, newUser._id);
   }
}

function findUserId(name, pass, callback)
{
   var hash = crypto.createHash('sha1');
   hash.update(pass);
   var sha1 = hash.digest('hex');
   UserModel.findOne({ name: name, sha1pass: sha1 }, { _id: 1 }, function(err, user)
   {
      if (user) callback(err, user._id);
      else callback(err, null);
   });
}

function getUser(id, callback)
{
   UserModel.findOne({ _id: id }, callback);
}

function removeUser(id, callback)
{
   UserModel.remove({ _id: id }, callback);
}

function getSession(id, callback)
{
   SessionModel.findOne({ _id: id }, callback);
}

module.exports.tryAddUser = tryAddUser;
module.exports.findUserId = findUserId;
module.exports.getUser = getUser;
module.exports.removeUser = removeUser;
module.exports.getSession = getSession;

