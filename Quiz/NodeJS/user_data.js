var mongoose = require('mongoose');
var crypto = require('crypto');

var userSchema = mongoose.Schema({ name: String, sha1pass: String });
var UserModel = mongoose.model('User', userSchema);

var hash = crypto.createHash('sha1');

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
module.exports.removeUser = removeUser;

module.exports.displayAllUsers = displayAllUsers;
