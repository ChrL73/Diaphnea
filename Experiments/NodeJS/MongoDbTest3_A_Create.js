var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 10;
var ijArray;

var questionSchema = mongoose.Schema({ index: Number, text: String });
var questionListSchema;
var questionListModel;

var categorySchema = mongoose.Schema({ index: Number, name: String, questionList: mongoose.Schema.Types.ObjectId });
var testSchema;
var TestModel;

clearQuestionsLists();

function clearQuestionsLists()
{
   console.log("n=" + n1 + " clearQuestionsLists");
   
   questionListSchema = mongoose.Schema({ questions: [questionSchema] }, { collection: "questionLists" + n1.toString() });
   QuestionListModel = mongoose.model("QuestionList" + n1.toString(), questionListSchema);
   
   levelSchema = mongoose.Schema({ name: String, categories: [categorySchema] }, { collection: "levels" + n1.toString() });
   LevelModel = mongoose.model("Level" + n1.toString(), levelSchema);
   
   QuestionListModel.remove({}, clearLevels);
}

function clearLevels(err)
{
   if (err)
   {
      console.log(err);
   }
   else
   {
      LevelModel.remove({}, fillQuestionLists);
   }
}

function fillQuestionLists(err)
{
   if (err)
   {
      console.log(err);
   }
   else
   {
      ijArray = getIndex2Array(3, 6);
      
      aSync.forEachOf(ijArray, function(ij, l, callback)
      {
         var i = ij.i, j = ij.j;
         var questionList = new QuestionListModel();
         questionList.questions = [];
         
         var k;
         for (k = 0; k < (j + 1) * n1; ++k)
         {       
            var question = { index: k, text: "Question" + k.toString() };
            questionList.questions.push(question);
         }
         
         questionList.save(function(err, questionListDoc)
         {
            if (err)
            {
               console.log(err);
            }
            else
            {
               ijArray[l].questionlistId = questionListDoc._id;
               callback(false);
            }
         });
      }, fillLevels);
   }
}

function fillLevels(err)
{
   if (err)
   {
      console.log(err);
   }
   else
   {
      var level;
      
      aSync.forEachOf(ijArray, function(ij, l, callback)
      {
         var i = ij.i, j = ij.j;
         
         if (j == 0)
         {
            level = new LevelModel();
            level.name = "Level" + i.toString();
         }
         
         var category = { index: j, name: "Category" + j.toString(), questionList: ij.questionlistId };
         level.categories.push(category);
         
         level.save(function(err)
         {
            if (err)
            {
               console.log(err);
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
            console.log(err);
         }
         else
         {
            if (n1 < 1500)
            {
               n1 *= 2;
               clearQuestionsLists();
            }
            else
            {
               console.log("End");
            }
         }
      });
   }
}

function getIndex2Array(n, m)
{
   var i, j, array = [];
   for (i = 0; i < n; ++i) for (j = 0; j < m; ++j) array.push({ i: i, j: j });
   return array;
}
