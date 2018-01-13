var mongoose = require("mongoose");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 100;
var i = 0;
var iMax = 100;
var t0;

var scoreSchema;
var ScoreModel;

f1();

function f1()
{
   console.log("n=" + n1);
   
   t0 = (new Date()).getTime();
   
   scoreSchema = mongoose.Schema({ _id: Number, name: String, score: Number, time_ms: Number, date: { type: Date, default: Date.now, expires: 3600 * 24 } }, { collection: "score" + n1.toString() });
   ScoreModel = mongoose.model("Score" + n1.toString(), scoreSchema);
   
   f2();
}

function f2()
{
   // The goal of this test is:
   // Verifying that the execution time of the folowing query stays constant when the collection size grows.
   ScoreModel.find().limit(10).sort('_id').exec(function(err, scores)
   {
      if (err)
      {
         console.log(err);
         process.exit();
      }
      
      //if (i == 0) console.log(scores);
      
      ++i;
      if (i >= iMax)
      {
         console.log("dt=" + ((new Date()).getTime() - t0) + "ms");
         
         if (n1 < 30000)
         {
            i = 0;
            n1 *= 2;
            setTimeout(f1, 0);
         }
         else
         {
            console.log("End");
            process.exit();
         }
      }
      else
      {
         setTimeout(f2, 0);
      }
   });
}
