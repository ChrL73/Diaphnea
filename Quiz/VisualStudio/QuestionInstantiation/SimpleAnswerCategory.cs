using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class SimpleAnswerCategory : Category
    {
        //private readonly List<PossibleAnswer> _answerList = new List<PossibleAnswer>();
        private readonly Dictionary<string, List<PossibleAnswer>> _answerDictionary = new Dictionary<string, List<PossibleAnswer>>();
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection) : base(weightIndex)
        {
            _proximityCriterion = proximityCriterion;
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal int DistinctAnswerCount
        {
            get { return _answerDictionary.Count; }
        }

        /*internal int TotalAnswerCount
        {
            get { return _answerList.Count; }
        }*/

        internal void addAnswer(PossibleAnswer answer)
        {
            //_answerList.Add(answer);

            // TODO: The following code is not correct: the answer should not be added in the following case: 2 answer texts are not identical in a language, but identical in another language
            string answerTextId = answer.AttributeValue.Value.TextId;
            if (!_answerDictionary.ContainsKey(answerTextId)) _answerDictionary.Add(answerTextId, new List<PossibleAnswer>());
            _answerDictionary[answerTextId].Add(answer);
        }

        internal void addQuestion(SimpleAnswerQuestion question)
        {
            _questionList.Add(question);
        }

        internal int setComments(XmlAttributeType attributeType, QuizData quizData)
        {
            foreach (List<PossibleAnswer> answerList in _answerDictionary.Values)
            {
                List<Text> textList = new List<Text>();
                foreach (PossibleAnswer answer in answerList)
                {
                    if (attributeType == null)
                    {
                        textList.Add(answer.Element.Name);
                    }
                    else
                    {
                        AttributeValue attributeValue = answer.Element.getAttributeValue(attributeType);
                        if (attributeValue != null) textList.Add(attributeValue.Value);
                    }
                }

                Text commentText = new Text();
                foreach (XmlLanguage xmlLanguage in quizData.XmlQuizData.parameters.languageList.Where(x => x.status == XmlLanguageStatusEnum.TRANSLATION_COMPLETED))
                {
                    List<string> list = new List<string>();
                    foreach (Text text in textList) list.Add(text.getText(xmlLanguage.id));
                    string comment = String.Join(", ", list);
                    commentText.setText(xmlLanguage.id, comment);
                }

                foreach (PossibleAnswer answer in answerList) answer.Comment = commentText;
            }

            return 0;
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database)
        {
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");

            BsonDocument questionListDocument = getQuestionListDocument();
            questionListsCollection.InsertOne(questionListDocument);

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "question_list", questionListDocument.GetValue("_id") },
                { "distrib_parameter_correction", _distribParameterCorrection }
            };

            categoryDocument.AddRange(getAnswerListDocument());

            return categoryDocument;
        }

        internal BsonDocument getQuestionListDocument()
        {
            BsonDocument questionListDocument = new BsonDocument();

            BsonArray questionsArray = new BsonArray();
            foreach (SimpleAnswerQuestion question in _questionList)
            {
                questionsArray.Add(question.getBsonDocument());
            }

            BsonDocument questionsDocument = new BsonDocument()
            {
                { "questions", questionsArray }
            };

            questionListDocument.AddRange(questionsDocument);

            return questionListDocument;
        }
        
        internal BsonDocument getAnswerListDocument()
        {
            BsonDocument answerListDocument = new BsonDocument();

            BsonArray answersArray = new BsonArray();
            foreach (List<PossibleAnswer> list in _answerDictionary.Values)
            {
                string proximityCriterionValue = "";
                if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
                {
                    proximityCriterionValue = list[0].Element.XmlElement.sortKey;
                }
                else
                {
                    throw new NotImplementedException();
                }

                BsonDocument answerDocument = new BsonDocument()
                {
                    { "answer_text", list[0].AttributeValue.Value.getBsonDocument() },
                    { "proximity_criterion_value", proximityCriterionValue }
                };
                answersArray.Add(answerDocument);
            }

            string proximityCriterionType = "none";
            if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
            {
                proximityCriterionType = "string";
            }
            else
            {
                throw new NotImplementedException();
            }

            BsonDocument answersDocument = new BsonDocument()
            {
                { "proximity_criterion_type", proximityCriterionType},
                { "answers", answersArray }
            };

            answerListDocument.AddRange(answersDocument);

            return answerListDocument;
        }
    }
}
