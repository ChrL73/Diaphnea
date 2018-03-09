var fr = require('./languages/fr');
var en = require('./languages/en');
//var fr2 = require('./languages/fr2');

module.exports = function(languageId, textId)
{
   if (!languageId) return { languages:
   [
      { id: 'fr', name: 'Français'},
      { id: 'en', name: 'English'},
      //{ id: 'fr2', name: 'Français 2'}
   ] };
   
   if (!textId)
   {
      if (languageId == 'fr') return { texts: fr };
      if (languageId == 'en') return { texts: en };
      //if (languageId == 'fr2') return { texts: fr2 };
   }
   
   if (languageId == 'fr') return fr[textId];
   if (languageId == 'en') return en[textId];
   //if (languageId == 'fr2') return fr2[textId];
};
