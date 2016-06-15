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
      var ijArray = getIndex2Array(3, 6);
      
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
            ijArray[l].questionlistId = questionListDoc._id;
            callback(false);
         });
      },
      function(err)
      {
         console.log(ijArray);
      });
   }
}

function fillLevels(err)
{
   
}

/*function fillArray(err)
{
   if (err)
   {
      console.log(err);
   }
   else
   {
      console.log("n=" + n1 + " fillArray");

      var iArray = getIndexArray(3);
      
      aSync.each(iArray, function(i, callback)
      {
         var level = new LevelModel();
         level.name = "Level" + i.toString();

         var j;
         for (j = 0; j < 6; ++j)
         {
            var category = { index: j, name: "Category" + j.toString(), questions: [] };
            
            var k;
            for (k = 0; k < (j + 1) * n1; ++k)
            {
               var question = { index: k, text: "Question" + k.toString() };
               category.questions.push(question);
            }
            
            level.categories.push(category);
         }

         level.save(function() { callback(false); });
      },
      function(err)
      {
         if (err)
         {
            console.log("err = " + err);
         }
         else
         {
            if (n1 < 10);//1500)
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
}*/

function getIndexArray(n)
{
   var i, array = [];
   for (i = 0; i < n; ++i) array.push(i);
   return array;
}

function getIndex2Array(n, m)
{
   var i, j, array = [];
   for (i = 0; i < n; ++i) for (j = 0; j < m; ++j) array.push({ i: i, j: j });
   return array;
}
