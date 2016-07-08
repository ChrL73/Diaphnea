var translate = require('./translate');
var languages = translate().languages;

var mongoose = require('mongoose');

var languageSchema = mongoose.Schema({ id: String, name: String });
var questionnaireSchema = mongoose.Schema({ questionnaire: String, name: mongoose.Schema.Types.Mixed, languages: [languageSchema] });
var QuestionnaireModel = mongoose.model('Questionnaire', questionnaireSchema);

var levelSchema = mongoose.Schema({ questionnaire: String, levelId: String, name: mongoose.Schema.Types.Mixed, categories: mongoose.Schema.Types.Mixed });
var LevelModel = mongoose.model('Level', levelSchema);

function getLevelChoiceDownData(upData, callback)
{
   var questionnaire, language, level;
   var downData = { questionnaireList: [], languageList: [], levelList: [], showLanguageSelect: false };
   
   QuestionnaireModel.find().sort('questionnaire').exec(processQuestionnaires);
   
   function processQuestionnaires(err, questionnaires)
   {
      var defaultQuestionnaire;
      questionnaires.forEach(function(iQuestionnaire)
      {
         if (!defaultQuestionnaire) defaultQuestionnaire = iQuestionnaire; 
         if (iQuestionnaire.questionnaire === upData.questionnaireId) questionnaire = iQuestionnaire;
      });
      
      if (!questionnaire) questionnaire = defaultQuestionnaire;
      
      var defaultLanguage, siteLanguage;
      questionnaire.languages.forEach(function(iLanguage)
      {
         if (!defaultLanguage) defaultLanguage = iLanguage;
         if (iLanguage.id === upData.siteLanguageId ) siteLanguage = iLanguage;
         if (iLanguage.id === upData.languageId) language = iLanguage;
         downData.languageList.push({ id: iLanguage.id, name: iLanguage.name });
      });
      
      // If the 'questionnaire language list' and the 'site language list' contain exactly the same languages,
      // the user does not choose the questionnaire language: The same language is used for the site and for the questionnaire.
      // Otherwise, the user can choose a different languuage for the questionnaire than for the site
      if (languages.length == downData.languageList.length)
      {
         var array1 = [];
         languages.forEach(function(language) { array1.push(language.id); } )
         array1.sort();
         
         var array2 = [];
         downData.languageList.forEach(function(language) { array2.push(language.id); } )
         array2.sort();
         
         var i, n = languages.length;
         for (i = 0; i < n; ++i)
         {
            if (array1[i] != array2[i])
            {
               downData.showLanguageSelect = true;
               break;
            }
         }
      }
      else
      {
         downData.showLanguageSelect = true;
      }
      
      if (siteLanguage && !downData.showLanguageSelect) language = siteLanguage;
      if (!language) language = defaultLanguage;
      
      questionnaires.forEach(function(iQuestionnaire)
      {
         var questionnaireName = iQuestionnaire.name[language.id];
         if (!questionnaireName) questionnaireName = iQuestionnaire.name[iQuestionnaire.languages[0].id];
         downData.questionnaireList.push({id: iQuestionnaire.questionnaire, name: questionnaireName });
      });
      
      LevelModel.find({ questionnaire: questionnaire.questionnaire }).sort('index').exec(processLevels);
   }
   
   function processLevels(err, levels)
   {
      var defaultLevel;
      
      levels.forEach(function(iLevel)
      {
         if (!defaultLevel) defaultLevel = iLevel;
         if (iLevel.levelId === upData.levelId) level = iLevel;
         downData.levelList.push({ id: iLevel.levelId, name: iLevel.name[language.id] });
      });
      
      if (!level) level = defaultLevel;
      
      downData.questionnaireId = questionnaire.questionnaire;
      downData.languageId = language.id;
      downData.levelId = level.levelId;
      
      callback(downData);
   }
}

module.exports.getLevelChoiceDownData = getLevelChoiceDownData;
