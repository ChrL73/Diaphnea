﻿using MongoDB.Bson;
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
        private readonly SortedDictionary<Text, List<PossibleAnswer>> _answerDictionary = new SortedDictionary<Text, List<PossibleAnswer>>(new TextComparer());
        private readonly List<SimpleAnswerQuestion> _questionList = new List<SimpleAnswerQuestion>();
        private readonly XmlAnswerProximityCriterionEnum _proximityCriterion;
        private readonly double _distribParameterCorrection;

        internal SimpleAnswerCategory(Int32 weightIndex, XmlAnswerProximityCriterionEnum proximityCriterion, double distribParameterCorrection, string questionNameInLog)
            : base(weightIndex, questionNameInLog)
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

            Text answerText = answer.AttributeValue.Value;
            if (!_answerDictionary.ContainsKey(answerText)) _answerDictionary.Add(answerText, new List<PossibleAnswer>());
            _answerDictionary[answerText].Add(answer);
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
                    foreach (Text text in textList) list.Add(text.getText(xmlLanguage.id.ToString()));
                    string comment = String.Join(", ", list);
                    commentText.setText(xmlLanguage.id.ToString(), comment);
                }

                foreach (PossibleAnswer answer in answerList) answer.Comment = commentText;
            }

            return 0;
        }

        internal override BsonDocument getBsonDocument(IMongoDatabase database, string questionnaireId)
        {
            IMongoCollection<BsonDocument> questionListsCollection = database.GetCollection<BsonDocument>("question_lists");

            BsonDocument questionListDocument = getQuestionListDocument(questionnaireId);
            questionListsCollection.InsertOne(questionListDocument);

            BsonDocument categoryDocument = new BsonDocument()
            {
                { "type", "SimpleAnswer" },
                { "question_list", questionListDocument.GetValue("_id") },
                { "distrib_parameter_correction", _distribParameterCorrection }
            };

            categoryDocument.AddRange(getAnswerListDocument());

            return categoryDocument;
        }

        internal BsonDocument getQuestionListDocument(string questionnaireId)
        {
            BsonDocument questionListDocument = new BsonDocument();

            BsonArray questionsArray = new BsonArray();
            foreach (SimpleAnswerQuestion question in _questionList)
            {
                questionsArray.Add(question.getBsonDocument());
            }

            BsonDocument questionsDocument = new BsonDocument()
            {
                { "questionnaire", questionnaireId },
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
                BsonArray proximityCriterionArray = new BsonArray();
                if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
                {
                    foreach (PossibleAnswer possibleAnswer in list) proximityCriterionArray.Add(possibleAnswer.Element.XmlElement.sortKey);
                }
                else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
                {
                    foreach (PossibleAnswer possibleAnswer in list)
                    {
                        string s = possibleAnswer.AttributeValue.Value.getAsNumber();
                        if (s == null)
                        {
                            MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format("Category \"{0}\" (with answerProximityCriterion=\"ATTRIBUTE_VALUE_AS_NUMBER\"), answer {1}: Fail to convert attribute value to number, value 0 is used as proximity criterion",
                                QuestionNameInLog, possibleAnswer.Element.XmlElement.id));
                            proximityCriterionArray.Add("0");
                        }
                        else
                        {
                            proximityCriterionArray.Add(s);
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                BsonDocument answerDocument = new BsonDocument()
                {
                    { "answer_text", list[0].AttributeValue.Value.getBsonDocument() },
                    { "answer_comment", list[0].Comment.getBsonDocument() },
                    { "proximity_criterion_values", proximityCriterionArray }
                };
                answersArray.Add(answerDocument);
            }

            string proximityCriterionType = "none";
            if (_proximityCriterion == XmlAnswerProximityCriterionEnum.SORT_KEY)
            {
                proximityCriterionType = "string";
            }
            else if (_proximityCriterion == XmlAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
            {
                proximityCriterionType = "number";
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
