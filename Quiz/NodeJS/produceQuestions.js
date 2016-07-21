var crypto = require('crypto');

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');

var levelSchema = 
{
   questionnaire: String,
   level_id: String,
   index: Number,
   name: mongoose.Schema.Types.Mixed,
   weight_sum: Number,
   category_count: Number,
   categories: [mongoose.Schema.Types.Mixed]
};
var Level = mongoose.model('Level', levelSchema);

process.stdin.on('data', function(jsonData)
{
   var parameters = JSON.parse(jsonData);
   var data = {};
   
   Level.findOne({ questionnaire: parameters.questionnaireId, level_id: parameters.levelId }, { weight_sum: 1, category_count: 1 }, function(err, level)
   {
      // Todo: handle error
      
      // Todo: category 'suitability'
      
      var draw = getRandomInt(level.weight_sum);
      /*var categoryCount = level.categories.length;
      int i;
      for (i = 0; i < categoryCount)
      {
      
      }*/
      
      data.level = level;
      
      process.stdout.write(JSON.stringify(data), function() { process.exit(); });    
   });
});
   
function getRandomInt(valueCount)
{
   var buf = crypto.randomBytes(32);
   var i = buf.readUInt32LE(0);
   return i % valueCount;
}
