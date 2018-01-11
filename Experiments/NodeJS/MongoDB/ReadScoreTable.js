var mongoose = require("mongoose");

var db = mongoose.connect("mongodb://localhost/testDb");

var scoreSchema = mongoose.Schema({ _id: Number, name: String, score: Number, time_ms: Number });
var ScoreModel = mongoose.model('Score', scoreSchema);

ScoreModel.find().limit(30).sort('_id').exec(function(err, scores)
{
   console.log(scores);
});
