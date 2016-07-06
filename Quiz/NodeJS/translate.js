var fr = require('./languages/fr');
var en = require('./languages/en');

module.exports = function(languageId, textId)
{
   if (!languageId) return { languages:
   [
      { id: 'fr', name: 'Français'} ,
      { id: 'en', name: 'English'}
   ] };
   
   if (!textId)
   {
      if (languageId == 'fr') return { texts: fr };
      if (languageId == 'en') return { texts: en };
   }
   
   if (languageId == 'fr') return fr[textId];
   if (languageId == 'en') return en[textId];
};
