var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 10;

var questionSchema = mongoose.Schema({ index: Number, text: String });
var questionListSchema;
var questionListModel;

var categorySchema = mongoose.Schema({ index: Number, name: String, questionList: mongoose.Schema.Types.ObjectId });
var testSchema;
var TestModel;

testPerf();

function testPerf()
{
   questionListSchema = mongoose.Schema({ questions: [questionSchema] }, { collection: "questionLists" + n1.toString() });
   QuestionListModel = mongoose.model("QuestionList" + n1.toString(), questionListSchema);
   
   levelSchema = mongoose.Schema({ name: String, categories: [categorySchema] }, { collection: "levels" + n1.toString() });
   LevelModel = mongoose.model("Level" + n1.toString(), levelSchema);
   
   var i = 1, j = 2, k = 5;
   
   //LevelModel.findOne({ name: "Level" + i.toString() }, { categories: { $elemMatch: { index: j } } }, function(err, level)
   LevelModel.findOne({ name: "Level" + i.toString() }, { categories: { $slice: [j, 1] } }, function(err, level)
   {
      if (err || !level)
      {
         console.log("err: " + err + " level: " + level);
      }
      else
      {
         console.log(level);
         var questionListId = level.categories[0].questionList;

         QuestionListModel.findOne({ _id: questionListId }, { questions: { $slice: [k, 1] } }, function(err, questionList)
         {
            if (err || !questionList)
            {
               console.log("err: " + err + " questionList: " + questionList);
            }
            else
            {
               console.log(questionList);
            }
         });
      }
   });
}
