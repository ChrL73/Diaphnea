var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var questionSchema, categorySchema, testSchema;
var TestModel;

function testPerf(field)
{
   questionSchema = mongoose.Schema({ index: Number, text: String });
   categorySchema = mongoose.Schema({ index: Number, name: String, questions: [questionSchema] });
   levelSchema = mongoose.Schema({ name: String, categories: [categorySchema] }, { collection: "levels" + n1.toString() });
   LevelModel = mongoose.model("Level" + n1.toString(), levelSchema);
   
   
}
