/*var randomIntGenerator = require('./random_int_generator');
var mongoose = require('mongoose');

var questionSchema = mongoose.Schema(
{
   question: mongoose.Schema.Types.Mixed,
   answer: mongoose.Schema.Types.Mixed,
   comment: mongoose.Schema.Types.Mixed
});

var questionListSchema = mongoose.Schema(
{
   questionnaire: String,
   count: Number,
   questions: [questionSchema]
});

var QuestionList = mongoose.model('Question_list', questionListSchema);

module.exports = function produceSimpleAnswerQuestion(category, languageId,  callback)
{
   var questionCount;
   
   QuestionList.findOne({ _id: category.question_list }, { count: 1 }, f1); 
   
   function f1(err, questionList)
   {
      // Todo: handle error
      questionCount = questionList.count;
      var draw = randomIntGenerator.getRandomInt(questionCount);

      QuestionList.findOne({ _id: category.question_list }, { questions: { $slice: [draw, 1] } }, f2);
   }
   
   function f2(err, questionList)
   {
      callback(questionList.questions[0].question[languageId]);
   }
}*/
