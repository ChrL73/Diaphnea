var mongoose = require("mongoose");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 100;

var scoreSchema;
var ScoreModel;

// (scoreSize + timeSize + randPartSize) must be <= 52 (because JavaScript numbers are 64-bit floating point numbers, with 52-bit mantissa)
var scoreSize = 6; // Assumes that the number of questions in a questionnaire is <= 63 (= 2^6 - 1)
var timeSize = 28; // Assumes that the duration of a game is <= 2^28ms (~74 days)
var randPartSize = 18;
var maxScore = Math.pow(2, scoreSize) - 1;
var scoreShift = Math.pow(2, timeSize + randPartSize);
var timeShift = Math.pow(2, randPartSize);
var maxRandPart = Math.pow(2, randPartSize) - 1;

var count = 0;

f1();

function f1()
{
   console.log("n=" + n1);
   
   scoreSchema = mongoose.Schema({ _id: Number, name: String, score: Number, time_ms: Number, date: { type: Date, default: Date.now, expires: 3600 * 24 } }, { collection: "score" + n1.toString() });
   ScoreModel = mongoose.model("Score" + n1.toString(), scoreSchema);
     
   ScoreModel.remove({}, f2);
}

function f2()
{
   var score;

   var i;
   for (i = 0; i < n1; ++i)
   {
      score = new ScoreModel();
      score.name = 'a';
      score.score = randomInt(maxScore);
      score.time_ms = randomInt(10);
      score.retryCount = 0;

      setIdAndSave(score);
   }
   
   setTimeout(f3, 100);
}

function f3()
{
   if (count == n1)
   {
      if (n1 < 30000)
      {
         count = 0;
         n1 *= 2;
         f1();
      }
      else
      {
         console.log("End");
         process.exit();
      }
   }
   else
   {
      setTimeout(f3, 100);
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
      else
      {
         ++count;
      }
   });
}

function randomInt(max)
{
   return Math.floor(Math.random() * Math.floor(max + 1));
}
