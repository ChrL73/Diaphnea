var mongoose = require("mongoose");

var db = mongoose.connect("mongodb://localhost/testDb");

var scoreSchema = mongoose.Schema({ _id: Number, name: String, score: Number, time_ms: Number, date: { type: Date, default: Date.now, expires: 3600 * 24 } });
var ScoreModel = mongoose.model('Score', scoreSchema);

// (scoreSize + timeSize + randPartSize) must be <= 52 (because JavaScript numbers are 64-bit floating point numbers, with 52-bit mantissa)
var scoreSize = 6; // Assumes that the number of questions in a questionnaire is <= 63 (= 2^6 - 1)
var timeSize = 28; // Assumes that the duration of a game is <= 2^28ms (~74 days)
var randPartSize = 18;
var maxScore = Math.pow(2, scoreSize) - 1;
var scoreShift = Math.pow(2, timeSize + randPartSize);
var timeShift = Math.pow(2, randPartSize);
var maxRandPart = Math.pow(2, randPartSize) - 1;

ScoreModel.remove({}, f);

function f()
{
   var score;

   var i;
   for (i = 0; i < 1000; ++i)
   {
      score = new ScoreModel();
      score.name = 'a';
      score.score = randomInt(maxScore);
      score.time_ms = randomInt(10);
      score.retryCount = 0;

      setIdAndSave(score);
   }
}

function setIdAndSave(score)
{
   score._id = (maxScore - score.score) * scoreShift + score.time_ms * timeShift + randomInt(maxRandPart);
   
   score.save(function(err)
   {
      if (err)
      {
         if (err.code == 11000 && score.retryCount < 10)
         {
            ++score.retryCount;
            console.log(score.retryCount)
            setIdAndSave(score);
         }
         else
         {
            console.log(err);
         }
      }
   });
}

function randomInt(max)
{
   return Math.floor(Math.random() * Math.floor(max + 1));
}
