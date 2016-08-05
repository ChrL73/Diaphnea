/*var randomIntGenerator = require('./random_int_generator');

var mongoose = require('mongoose');
var db = mongoose.connect('mongodb://localhost/diaphnea');

var levelSchema = 
{
   questionnaire: String,
   level_id: String,
   index: Number,
   name: mongoose.Schema.Types.Mixed,
   question_count: Number,
   choice_count: Number,
   weight_sum: Number,
   category_count: Number,
   categories: [mongoose.Schema.Types.Mixed]
};
var Level = mongoose.model('Level', levelSchema);

var produceSimpleAnswerQuestion = require('./produce_simple_answer_question');

var levelId = process.argv[2];
var languageId = process.argv[3];

Level.findOne({ _id: levelId }, function(err, level)
{
   // Todo: handle error
   // Todo: category 'suitability'

   var data = {};
   var questionIndex = 0;
   loop_part1();

   function loop_part1()
   {
      var draw = randomIntGenerator.getRandomInt(level.weight_sum);
      var i;
      for (i = 0; i < level.category_count; ++i)
      {
         if (draw < level.categories[i].weight_index) break;
      }

      var category = level.categories[i];
      if (category.type == 'SimpleAnswer') produceSimpleAnswerQuestion(category, languageId, loop_part2);
      else throw new Error("Unknown category type '" + category.type + "'");
   }

   function loop_part2(question)
   {
      data['question' + questionIndex] = question;

      ++questionIndex;
      if (questionIndex < level.question_count) loop_part1();
      else end();
   }

   function end()
   {
      console.log(JSON.stringify(data));
      process.exit();
   }
});*/
