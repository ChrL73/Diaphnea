var express = require('express');
var app = express();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var morgan = require('morgan');
var mongoose = require('mongoose');
var session = require('express-session');
//var sharedsession = require("express-socket.io-session");
//var cookieParser = require('cookie-parser');

app.use(express.static('public'));
app.use(morgan('dev'));

//app.use(cookieParser('ke7Hèq*fG5ùZ'));
app.use(session(
{
  secret: 'ke7Hèq*fG5ùZ',
  resave: true,
  saveUninitialized: true
}));
//io.use(sharedsession(session, { autoSave: true , resave: true,})); 

var db = mongoose.connect("mongodb://localhost/diaphnea");

var languageSchema = mongoose.Schema({ id: String, name: String });
var questionnaireSchema = mongoose.Schema({ questionnaire: String, name: mongoose.Schema.Types.Mixed, languages: [languageSchema] });
var QuestionnaireModel = mongoose.model("Questionnaire", questionnaireSchema);

var levelSchema = mongoose.Schema({ questionnaire: String, levelId: String, name: mongoose.Schema.Types.Mixed, categories: mongoose.Schema.Types.Mixed });
var LevelModel = mongoose.model("Level", levelSchema);

app.get('/', function(req, res)
{
   var sessionQuestionnaireId = req.session.questionnaireId;
   var sessionLanguageId = req.session.languageId;
   var sessionLevelId = req.session.levelId;
   /*var cookieQuestionnaireId = req.cookies.questionnaireId;
   var cookieLanguageId = req.cookies.languageId;
   var cookieLevelId = req.cookies.levelId;*/
   
   var questionnaire, language, level;
   var data = { questionnaireList: [], languageList: [], levelList: [] };
   
   console.log("sessionQuestionnaireId: " + sessionQuestionnaireId);
   console.log("sessionLanguageId: " + sessionLanguageId);
   console.log("sessionLevelId: " + sessionLevelId);
   /*console.log("cookieQuestionnaireId: " + cookieQuestionnaireId);
   console.log("cookieLanguageId: " + cookieLanguageId);
   console.log("cookieLevelId: " + cookieLevelId);*/
   
   QuestionnaireModel.find().sort("questionnaire").exec(processQuestionnaires);
   
   function processQuestionnaires(err, questionnaires)
   {
      var defaultQuestionnaire;
      questionnaires.forEach(function(iQuestionnaire)
      {
         if (!defaultQuestionnaire) defaultQuestionnaire = iQuestionnaire; 
         if (iQuestionnaire.questionnaire === sessionQuestionnaireId) questionnaire = iQuestionnaire;
         //if (iQuestionnaire.questionnaire === cookieQuestionnaireId) questionnaire = iQuestionnaire;
      });
      
      if (!questionnaire) questionnaire = defaultQuestionnaire;
      req.session.questionnaireId = questionnaire.questionnaire;
      //res.cookie('questionnaireId', questionnaire.questionnaire);
      
      var defaultLanguage;
      questionnaire.languages.forEach(function(iLanguage)
      {
         if (!defaultLanguage) defaultLanguage = iLanguage;
         if (iLanguage.id === sessionLanguageId) language = iLanguage;
         //if (iLanguage.id === cookieLanguageId) language = iLanguage;
         data.languageList.push(iLanguage.name);
      });
      
      if (!language) language = defaultLanguage;
      req.session.languageId = language.id;
      //res.cookie('languageId', language.id);
      
      questionnaires.forEach(function(iQuestionnaire)
      {
         data.questionnaireList.push(iQuestionnaire.name[language.id]);
      });
      
      LevelModel.find({ questionnaire: questionnaire.questionnaire }, processLevels);
   }
   
   function processLevels(err, levels)
   {
      var defaultLevel;
      
      levels.forEach(function(iLevel)
      {
         if (!defaultLevel) defaultLevel = iLevel;
         if (iLevel.levelId === sessionLevelId) level = iLevel;
         //if (iLevel.levelId === cookieLevelId) level = iLevel;
         data.levelList.push(iLevel.name[language.id]);
      });
      
      if (!level) level = defaultLevel;
      req.session.levelId = level.levelId;
      //res.cookie('levelId', level.levelId);
      
      renderView();
   }
   
   function renderView()
   {
      res.render('index.ejs', { test: questionnaire.questionnaire, data: data });
   }
});

io.on('connection', function(socket)
{
   socket.on('questionnaireChoice', function(questionnaireName)
   {
      console.log(questionnaireName);
      //console.log(socket.handshake);
      //console.log(socket.handshake.headers.cookie);
   });
});

console.log('Quiz server listening on port 3000...');
server.listen(3000);
