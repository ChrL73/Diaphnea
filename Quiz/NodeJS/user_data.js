var mongoose = require('mongoose');
var crypto = require('crypto');

/*var choiceSchema = mongoose.Schema(
{
   text: String,
   comment: String,
   isRight: Boolean,
   isChecked: Boolean
});

var questionSchema = mongoose.Schema(
{
   question: String,
   isMultiple: Boolean,
   answerReceived: Boolean,
   choices: [choiceSchema]
});*/

var userSchema = mongoose.Schema(
{
   name: String,
   sha1pass: String,
   parameters: { siteLanguageId: String, questionnaireId: String, languageId: String, levelId: String },
   gameState: { displayedQuestion: Number, questions: mongoose.Schema.Types.Mixed /*[questionSchema]*/ }
});

var UserModel = mongoose.model('User', userSchema);

function tryAddUser(name, pass, callback)
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
         callback(null, null);
         return;
      }
      
      var hash = crypto.createHash('sha1');
      hash.update(pass);
      var sha1 = hash.digest('hex');
      
      newUser = new UserModel();
      newUser.name = name;
      newUser.sha1pass = sha1;
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

function updateParameters(user, parameters, callback)
{
   if (!user.parameters) user.parameters = {};
   
   if (parameters.siteLanguageId) user.parameters.siteLanguageId = parameters.siteLanguageId;
   if (parameters.questionnaireId) user.parameters.questionnaireId = parameters.questionnaireId;
   if (parameters.languageId) user.parameters.languageId = parameters.languageId;
   if (parameters.levelId) user.parameters.levelId = parameters.levelId;
   
   user.save(callback);
}

function removeUser(id, callback)
{
   UserModel.remove({ _id: id }, callback);
}

function displayAllUsers()
{
   UserModel.find({}, function(err, users)
   {
      users.forEach(function(user)
      {
         console.log(user);
      });
   });
}

module.exports.tryAddUser = tryAddUser;
module.exports.findUserId = findUserId;
module.exports.getUser = getUser;
module.exports.updateParameters = updateParameters;
module.exports.removeUser = removeUser;

module.exports.displayAllUsers = displayAllUsers;
