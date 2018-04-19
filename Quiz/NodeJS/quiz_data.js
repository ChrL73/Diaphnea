var translate = require('./translate');
var languages = translate().languages;

var questionnaires = require('./generated_data');

function getLevelChoiceDownData(upData, callback)
{
   var questionnaire, questionnaireLanguage, level;
   var downData = { questionnaireList: [], questionnaireLanguageList: [], levelList: [], showQuestionnaireLanguageSelect: false };
   
   var defaultQuestionnaire;
   questionnaires.forEach(function(iQuestionnaire)
   {
      if (!defaultQuestionnaire) defaultQuestionnaire = iQuestionnaire; 
      if (iQuestionnaire.id === upData.questionnaireId) questionnaire = iQuestionnaire;
   });

   if (!questionnaire) questionnaire = defaultQuestionnaire;

   var defaultLanguage, siteLanguage;
   questionnaire.languages.forEach(function(iLanguage)
   {
      if (!defaultLanguage) defaultLanguage = iLanguage;
      if (iLanguage.id === upData.siteLanguageId ) siteLanguage = iLanguage;
      if (iLanguage.id === upData.questionnaireLanguageId) questionnaireLanguage = iLanguage;
      downData.questionnaireLanguageList.push({ id: iLanguage.id, name: iLanguage.name });
   });

   // If the 'questionnaire language list' and the 'site language list' contain exactly the same languages,
   // the user does not choose the questionnaire language: The same language is used for the site and for the questionnaire.
   // Otherwise, the user can choose a different language for the questionnaire than for the site
   if (languages.length == downData.questionnaireLanguageList.length)
   {
      var array1 = [];
      languages.forEach(function(language) { array1.push(language.id); } )
      array1.sort();

      var array2 = [];
      downData.questionnaireLanguageList.forEach(function(language) { array2.push(language.id); } )
      array2.sort();

      var i, n = languages.length;
      for (i = 0; i < n; ++i)
      {
         if (array1[i] != array2[i])
         {
            downData.showQuestionnaireLanguageSelect = true;
            break;
         }
      }
   }
   else
   {
      downData.showQuestionnaireLanguageSelect = true;
   }

   if (siteLanguage && !downData.showQuestionnaireLanguageSelect) questionnaireLanguage = siteLanguage;
   if (!questionnaireLanguage) questionnaireLanguage = defaultLanguage;

   questionnaires.forEach(function(iQuestionnaire)
   {
      var questionnaireName = iQuestionnaire.name[questionnaireLanguage.id];
      if (!questionnaireName) questionnaireName = iQuestionnaire.name[iQuestionnaire.languages[0].id];
      downData.questionnaireList.push({ id: iQuestionnaire.id, name: questionnaireName });
   });

   var levels = questionnaire.levels;
   var defaultLevel;

   levels.forEach(function(iLevel)
   {
      if (!defaultLevel) defaultLevel = iLevel;
      if (iLevel.id === upData.levelId) level = iLevel;
      downData.levelList.push({ id: iLevel.id, name: iLevel.name[questionnaireLanguage.id] });
   });

   if (!level) level = defaultLevel;

   downData.questionnaireId = questionnaire.id;
   downData.questionnaireName = questionnaire.name[questionnaireLanguage.id];
   downData.questionnaireLanguageId = questionnaireLanguage.id;
   downData.levelId = level.id;
   downData.levelName = level.name[questionnaireLanguage.id];
   downData.mapId = questionnaire.mapId;

   callback(downData);
}

module.exports.getLevelChoiceDownData = getLevelChoiceDownData;
