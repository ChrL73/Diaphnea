var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 100;
var i = 1, j = 0;

var questionSchema = mongoose.Schema({ index: Number, text: String });
var questionListSchema;
var questionListModel;

var categorySchema = mongoose.Schema({ index: Number, name: String, questionList: mongoose.Schema.Types.ObjectId });
var testSchema;
var TestModel;

findQuestionList();

function findQuestionList()
{
   console.log("n=" + n1 + " findQuestionList");
   
   questionListSchema = mongoose.Schema({ questions: [questionSchema] }, { collection: "questionLists" + n1.toString() });
   QuestionListModel = mongoose.model("QuestionList" + n1.toString(), questionListSchema);
   
   levelSchema = mongoose.Schema({ name: String, categories: [categorySchema] }, { collection: "levels" + n1.toString() });
   LevelModel = mongoose.model("Level" + n1.toString(), levelSchema);
   
   LevelModel.findOne({ name: "Level" + i.toString() }, { categories: { $slice: [j, 1] } }, function(err, level)
   {
      if (err || !level)
      {
         console.log("err: " + err + " level: " + level);
      }
      else
      {
         //console.log(level);
         var questionListId = level.categories[0].questionList;
         testPerf(questionListId);
      }
   });
}

function testPerf(questionListId)
{
   var k, n2 = 1000;
   var indexArray = [];
   for (k = 0; k < n2; ++k)
   {
      indexArray.push(Math.floor(Math.random() * N(i, j)));
   }
   
   var t0 = (new Date()).getTime();
   
   aSync.each(indexArray, function(k, callback)
   {
      // The goal of this test is:
      // Verifying that the execution time of the folowing query (findOne) stays constant when the array (questions) size grows.
      QuestionListModel.findOne({ _id: questionListId }, { questions: { $slice: [k, 1] } }, function(err, questionList)
      {
         if (err || !questionList)
         {
            console.log("err: " + err + " questionList: " + questionList);
            callback(true);
         }
         else
         {
            callback(false);
         }
      });
   },
   function(err)
   {
      if (err)
      {
         console.log("err = " + err);
      }
      else
      {
         console.log("dt=" + ((new Date()).getTime() - t0) + "ms");
         if (n1 < 30000)
         {
            n1 *= 2;
            setTimeout(findQuestionList(), 1);
         }
         else     
         {
            console.log("End of test");
            process.exit();
         }
      }
   });
}

function N(i, j)
{
   if (j == 0) return n1;
   return 10;
}
