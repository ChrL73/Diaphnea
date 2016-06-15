var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 10;

var questionSchema, categorySchema, testSchema;
var TestModel;

testPerf();

function testPerf()
{
   questionSchema = mongoose.Schema({ index: Number, text: String });
   categorySchema = mongoose.Schema({ index: Number, name: String, questions: [questionSchema] });
   levelSchema = mongoose.Schema({ name: String, categories: [categorySchema] }, { collection: "levels" + n1.toString() });
   LevelModel = mongoose.model("Level" + n1.toString(), levelSchema);
   
   LevelModel.findOne({ name: "Level1" }, { categories: { $elemMatch: { index: 2 } } }, function(err, level)
   {
      console.log(level);
      //console.log(level.categories[0]);
      //console.log(level.categories[0].questions[5].text);
   });
}
