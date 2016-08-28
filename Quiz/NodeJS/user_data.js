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

function getUser(id, callback)
{
   UserModel.findOne({ _id: id }, callback);
}

function getSession(id, callback)
{
   SessionModel.findOne({ _id: id }, callback);
}

module.exports.tryAddUser = tryAddUser;
module.exports.getUser = getUser;
module.exports.getSession = getSession;

