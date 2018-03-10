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
   scoreTab: Number,
   quizId: String,
   questions: mongoose.Schema.Types.Mixed,
   displayedQuestion: Number,
   questionStates: [stateSchema],
   rightAnswerCount: Number,
   answerCount: Number,
   startDate: Date,
   finalTime: Number,
   lastScore1: Number,
   lastScore7: Number,
   lastScore30: Number,
   lastScore365: Number,
   sendScoreWithSignUp: Boolean
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

var scoreModels = {};

function getScoreModel(questionnaireId, levelId, dayCount)
{
   var id = questionnaireId + levelId + dayCount;
   
   if (!scoreModels[id])
   {
      var schema = mongoose.Schema(
      {
         _id: Number,
         user_id: mongoose.Schema.Types.ObjectId,
         score: Number,
         time_ms: Number,
         date: { type: Date, expires: 3600 * 24 * dayCount }
      });
      scoreModels[id] = mongoose.model('Scores' + id, schema);
   }
   
   return scoreModels[id];
}

// (scoreSize + timeSize + randPartSize) must be <= 52 (because JavaScript numbers are 64-bit floating point numbers, with 52-bit mantissa)
var scoreSize = 6; // Assumes that the number of questions in a questionnaire is <= 63 (= 2^6 - 1)
var timeSize = 28; // Assumes that the duration of a game is <= 2^28ms (~74 days)
var randPartSize = 18;
var maxScore = Math.pow(2, scoreSize) - 1;
var scoreShift = Math.pow(2, timeSize + randPartSize);
var maxTime = Math.pow(2, timeSize) - 1;
var timeShift = Math.pow(2, randPartSize);
var maxRandPart = Math.pow(2, randPartSize) - 1;

function addScore(questionnaireId, levelId, user, score, time)
{
   var date = Date.now();
   
   if (score < 0 || score > maxScore) return;
   
   var time_ms = Math.floor(time * 1000);
   if (time_ms < 0 || time_ms > maxTime) return;
   
   var days = [1, 7, 30, 365];
   days.forEach(function(i)
   {   
      var model = getScoreModel(questionnaireId, levelId, i);
      addScore2(user, score, time_ms, date, model, i);
   });
}

function addScore2(user, score, time_ms, date, model, i)
{
   var entry = new model();
   entry.user_id = user._id;
   entry.score = score;  
   entry.time_ms = time_ms;
   entry.date = date;
   entry.retryCount = 0;
   
   setIdAndSave(user, entry, i);
}

function setIdAndSave(user, entry, i)
{
   entry._id = (maxScore - entry.score) * scoreShift + entry.time_ms * timeShift + randomInt(maxRandPart);
   
   entry.save(function(err)
   {
      if (err)
      {
         if (err.code == 11000 && entry.retryCount < 20)
         {
            ++entry.retryCount;
            setIdAndSave(entry);
         }
         else
         {
            console.log(err);
         }
      }
      else
      {
         user.context['lastScore' + i] = entry._id;
         user.save(function(err) { if (err) { console.log(err); /* Todo: handle error */ } });
      }
      
   });
}

function randomInt(max)
{
   return Math.floor(Math.random() * Math.floor(max + 1));
}

function getScoreTable(questionnaireId, levelId, dayCount, size, callerUser, callback)
{
   var table = [];
   var model = getScoreModel(questionnaireId, levelId, dayCount);
   
   if (model)
   {
      model.find().limit(size).sort('_id').exec(function(err, entries)
      {
         var i = 0;
         var n = entries.length;
         if (n == 0) callback(table);
         
         entries.forEach(function(entry, j)
         {
            getUser(entry.user_id, function(err, user)
            {
               var row =
               {
                  score: entry.score,
                  time_ms: entry.time_ms,
                  name: (!err && user ? user.name : ""),
               };
               
               if (!err && user && callerUser && String(user._id) == String(callerUser._id) && entry._id === user.context['lastScore' + dayCount]) row.highlight = true;
               table[j] = row;
               
               ++i;
               if (i == n) callback(table);
            });
         });
      });
   }
   else
   {
      console.log('!model');
      callback(table); 
   }
}

module.exports.tryAddUser = tryAddUser;
module.exports.findUserId = findUserId;
module.exports.getUser = getUser;
module.exports.removeUser = removeUser;
module.exports.getSession = getSession;
module.exports.addScore = addScore;
module.exports.getScoreTable = getScoreTable;
