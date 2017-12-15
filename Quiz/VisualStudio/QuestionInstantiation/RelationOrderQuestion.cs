using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class RelationOrderQuestion
    {
        private readonly List<Choice> _choiceList = new List<Choice>();
        private readonly SortedDictionary<Text, Choice> _choiceDictionary = new SortedDictionary<Text, Choice>(new TextComparer());
        private readonly Text _questionText;
        private readonly Element _questionElement;

        internal RelationOrderQuestion(Text questionText, Element questionElement)
        {
            _questionText = questionText;
            _questionElement = questionElement;
        }

        internal int ChoiceCount
        {
            get { return _choiceList.Count; }
        }

        internal void addChoice(Choice choice)
        {
            Text choiceText = choice.AttributeValue.Value;
            if (!_choiceDictionary.ContainsKey(choiceText))
	        {
                _choiceDictionary.Add(choiceText, choice);
                _choiceList.Add(choice);
	        }
	        else
	        {
                Choice choiceToDelete = _choiceDictionary[choiceText];
                int i, n = _choiceList.Count;
		        for (i = 0; i < n; ++i)
		        {
                    if (_choiceList[i] == choiceToDelete)
			        {
                        _choiceList.RemoveAt(i);
				        break;
			        }
		        }
	        }
        }

        internal BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId, QuizData quizData)
        {
            IMongoCollection<BsonDocument> choiceListsCollection = database.GetCollection<BsonDocument>("choice_lists");
            BsonDocument choiceListDocument = getChoiceListDocument(questionnaireId);
            choiceListsCollection.InsertOne(choiceListDocument);

            BsonDocument questionDocument = new BsonDocument()
            {
                { "question", _questionText.getBsonDocument() },
                { "choice_count", _choiceList.Count },
                { "choice_list", choiceListDocument.GetValue("_id") },
                { "map_id", (_questionElement.XmlElement.mapId == null) ? "" : _questionElement.XmlElement.mapId.Substring(2) }
            };

            return questionDocument;
        }

        private BsonDocument getChoiceListDocument(string questionnaireId)
        {
            BsonArray choicesArray = new BsonArray();
            foreach (Choice choice in _choiceList)
            {
                BsonDocument choiceDocument = new BsonDocument()
                {
                    { "choice", choice.AttributeValue.Value.getBsonDocument() },
                    { "map_id", choice.Element.XmlElement.mapId == null ? "" : choice.Element.XmlElement.mapId.Substring(2) }
                };

                choicesArray.Add(choiceDocument);
            }

            BsonDocument choicesDocument = new BsonDocument()
            {
                { "questionnaire", questionnaireId },
                { "count", _choiceDictionary.Count },
                { "choices", choicesArray }
            };

            BsonDocument choiceListDocument = new BsonDocument();
            choiceListDocument.AddRange(choicesDocument);

            return choiceListDocument;
        }
    }
}
