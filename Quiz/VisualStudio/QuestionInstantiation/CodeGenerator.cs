/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Schemas;

namespace QuestionInstantiation
{
    class CodeGenerator
    {
        private readonly string _dirName;
        private readonly string _languageId;

        private readonly Dictionary<string, StreamWriter> _streamWriterDictionary = new Dictionary<string, StreamWriter>();

        private int _currentStringOffset;
        private int _currentChoiceOffset;
        private int _currentIntArrayOffset;
        private int _currentDoubleArrayOffset;
        private int _currentSimpleAnswerQuestionOffset;
        private int _currentMapParametersOffset;
        private int _currentMapSubParametersOffset;
        private int _currentSimpleAnswerCategoryOffset;
        private int _currentMultipleAnswerQuestionOffset;
        private int _currentMultipleAnswerCategoryOffset;
        private int _currentRelationOrderQuestionOffset;
        private int _currentRelationOrderCategoryOffset;
        private int _currentAttributeOrderCategoryOffset;

        private readonly List<int> _simpleAnswerCategoryOffsets = new List<int>();
        private readonly List<int> _multipleAnswerCategoryOffsets = new List<int>();
        private readonly List<int> _relationOrderCategoryOffsets = new List<int>();
        private readonly List<int> _attributeOrderCategoryOffsets = new List<int>();

        private readonly Dictionary<string, int> _stringDictionary = new Dictionary<string, int>();

        internal CodeGenerator(string dirName, string languageId)
        {
            _dirName = dirName;
            _languageId = languageId;
            _currentStringOffset = 1;
            _currentChoiceOffset = 0;
            _currentIntArrayOffset = 1;
            _currentDoubleArrayOffset = 3;
            _currentSimpleAnswerQuestionOffset = 0;
            _currentMapParametersOffset = 0;
            _currentMapSubParametersOffset = 0;
            _currentSimpleAnswerCategoryOffset = 0;
            _currentMultipleAnswerQuestionOffset = 0;
            _currentRelationOrderQuestionOffset = 0;
            _currentRelationOrderCategoryOffset = 0;
            _currentAttributeOrderCategoryOffset = 0;

            _stringDictionary.Add("", 0);

            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            string path = String.Format("{0}/Choices.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("Choices.cpp", "namespace produce_questions\n{\nint choices[] =\n{");

            path = String.Format("{0}/DoubleArrays.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("DoubleArrays.cpp", "namespace produce_questions\n{\ndouble doubleArrays[] =\n{\n0,0,0");

            path = String.Format("{0}/IntArrays.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("IntArrays.cpp", "namespace produce_questions\n{\nint intArrays[] =\n{\n0");

            path = String.Format("{0}/Strings.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("Strings.cpp", "namespace produce_questions\n{\nunsigned char strings[] =\n{\n// 0: Empty string\n0");

            path = String.Format("{0}/SimpleAnswerQuestions.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("SimpleAnswerQuestions.cpp", "namespace produce_questions\n{\nint simpleAnswerQuestions[] =\n{");

            path = String.Format("{0}/MapParameterss.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("MapParameterss.cpp", "namespace produce_questions\n{\nint mapParameterss[] =\n{");

            path = String.Format("{0}/MapSubParameterss.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("MapSubParameterss.cpp", "namespace produce_questions\n{\nint mapSubParameterss[] =\n{");

            path = String.Format("{0}/SimpleAnswerCategories.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("SimpleAnswerCategories.cpp", "namespace produce_questions\n{\nint simpleAnswerCategories[] =\n{");

            path = String.Format("{0}/MultipleAnswerQuestions.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("MultipleAnswerQuestions.cpp", "namespace produce_questions\n{\nint multipleAnswerQuestions[] =\n{");

            path = String.Format("{0}/MultipleAnswerCategories.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("MultipleAnswerCategories.cpp", "namespace produce_questions\n{\nint multipleAnswerCategories[] =\n{");

            path = String.Format("{0}/RelationOrderQuestions.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("RelationOrderQuestions.cpp", "namespace produce_questions\n{\nint relationOrderQuestions[] =\n{");

            path = String.Format("{0}/RelationOrderCategories.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("RelationOrderCategories.cpp", "namespace produce_questions\n{\nint relationOrderCategories[] =\n{");

            path = String.Format("{0}/AttributeOrderCategories.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("AttributeOrderCategories.cpp", "namespace produce_questions\n{\nint attributeOrderCategories[] =\n{");
        }

        internal void close(int questionCount, int weightSum, double distribParameter, int choiceCount)
        {
            int simpleAnswerCategoryOffsets = 0;
            if (_simpleAnswerCategoryOffsets.Count() != 0)
            {
                simpleAnswerCategoryOffsets = getIntArrayOffset(_simpleAnswerCategoryOffsets);
            }

            int multipleAnswerCategoryOffsets = 0;
            if (_multipleAnswerCategoryOffsets.Count() != 0)
            {
                multipleAnswerCategoryOffsets = getIntArrayOffset(_multipleAnswerCategoryOffsets);
            }

            int relationOrderCategoryOffsets = 0;
            if (_relationOrderCategoryOffsets.Count() != 0)
            {
                relationOrderCategoryOffsets = getIntArrayOffset(_relationOrderCategoryOffsets);
            }

            int attributeOrderCategoryOffsets = 0;
            if (_attributeOrderCategoryOffsets.Count() != 0)
            {
                attributeOrderCategoryOffsets = getIntArrayOffset(_attributeOrderCategoryOffsets);
            }

            int[] distribParameterInt = doubleToIntArray(distribParameter);

            string path = String.Format("{0}/Level_.cpp", _dirName);
            if (File.Exists(path)) File.Delete(path);

            string code = String.Format(
                "namespace produce_questions\n{{\nint level[] =\n{{\n{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}\n}};\n}}\n",
                _simpleAnswerCategoryOffsets.Count(), simpleAnswerCategoryOffsets,
                _multipleAnswerCategoryOffsets.Count(), multipleAnswerCategoryOffsets,
                _relationOrderCategoryOffsets.Count(), relationOrderCategoryOffsets,
                _attributeOrderCategoryOffsets.Count(), attributeOrderCategoryOffsets,
                questionCount, weightSum, distribParameterInt[0], distribParameterInt[1], choiceCount);

            append("Level_.cpp", code);

            append("Choices.cpp", "\n};\n}\n");
            append("DoubleArrays.cpp", "\n};\n}\n");
            append("IntArrays.cpp", "\n};\n}\n");
            append("Strings.cpp", "\n};\n}\n");
            append("SimpleAnswerQuestions.cpp", "\n};\n}\n");
            append("MapParameterss.cpp", "\n};\n}\n");
            append("MapSubParameterss.cpp", "\n};\n}\n");
            append("SimpleAnswerCategories.cpp", "\n};\n}\n");
            append("MultipleAnswerQuestions.cpp", "\n};\n}\n");
            append("MultipleAnswerCategories.cpp", "\n};\n}\n");
            append("RelationOrderQuestions.cpp", "\n};\n}\n");
            append("RelationOrderCategories.cpp", "\n};\n}\n");
            append("AttributeOrderCategories.cpp", "\n};\n}\n");

            foreach (StreamWriter file in _streamWriterDictionary.Values) file.Close();
        }

        private int getStringOffset(string str)
        {
            int offset;
            if (!_stringDictionary.TryGetValue(str, out offset))
            {
                offset = _currentStringOffset;
                _stringDictionary.Add(str, _currentStringOffset);

                string code = String.Format(",\n// {0} \"{1}\"\n", _currentStringOffset, str);

                byte[] bytes = Encoding.UTF8.GetBytes(str);
                code += String.Join(",", bytes);
                code += ",0";

                _currentStringOffset += bytes.Count() + 1;

                append("Strings.cpp", code);
            }

            return offset;
        }

        internal int addSimpleAnswerChoice(List<Choice> choiceList, XmlSimpleAnswerProximityCriterionEnum proximityCriterion, string QuestionNameInLog)
        {
            string choiceText = choiceList[0].AttributeValue.Value.getText(_languageId);
            string comment = choiceList[0].Comment.getText(_languageId);
            string mapId = choiceList[0].Element.XmlElement.mapId == null ? "" : choiceList[0].Element.XmlElement.mapId.Substring(2);

            List<double> doubleCriterionValues = new List<double>();
            List<string> stringCriterionValues = new List<string>();
            List<double> pointCriterionValues = new List<double>();

            if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.SORT_KEY)
            {
                foreach (Choice choice in choiceList) stringCriterionValues.Add(choice.Element.XmlElement.sortKey);
            }
            else if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
            {
                foreach (Choice choice in choiceList)
                {
                    Double? d = choice.AttributeValue.Value.getAsDouble();
                    if (d == null)
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.WARNING, String.Format(
                            "Category \"{0}\" (with answerProximityCriterion=\"ATTRIBUTE_VALUE_AS_NUMBER\"), choice {1}: Fail to convert attribute value to number, value 0 is used as proximity criterion",
                            QuestionNameInLog, choice.Element.XmlElement.id));
                        doubleCriterionValues.Add(0);
                    }
                    else
                    {
                        doubleCriterionValues.Add(d.Value);
                    }
                }
            }
            else if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                foreach (Choice choice in choiceList)
                {
                    pointCriterionValues.Add(choice.Element.GeoPoint.X);
                    pointCriterionValues.Add(choice.Element.GeoPoint.Y);
                    pointCriterionValues.Add(choice.Element.GeoPoint.Z);
                }
            }

            int offset = _currentChoiceOffset;

            int choiceTextOffset = getStringOffset(choiceText);
            int commentOffset = getStringOffset(comment);
            int mapIdOffset = getStringOffset(mapId);
            int doubleCriterionValuesOffset = 0;
            if (doubleCriterionValues.Count != 0) doubleCriterionValuesOffset = getDoubleArrayOffset(doubleCriterionValues);

            int stringCriterionValuesOffset = 0;
            if (stringCriterionValues.Count != 0)
            {
                List<int> stringOffsets = new List<int>();
                foreach (string str in stringCriterionValues)
                {
                    stringOffsets.Add(getStringOffset(str));
                }

                stringCriterionValuesOffset = getIntArrayOffset(stringOffsets);
            }

            int pointCriterionValuesOffset = 0;
            if (pointCriterionValues.Count != 0) pointCriterionValuesOffset = getDoubleArrayOffset(pointCriterionValues);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11}", offset == 0 ? "" : ",", _currentChoiceOffset, choiceText,
                choiceTextOffset, commentOffset, mapIdOffset, doubleCriterionValues.Count, doubleCriterionValuesOffset, stringCriterionValues.Count,
                stringCriterionValuesOffset, pointCriterionValues.Count / 3, pointCriterionValuesOffset);

            append("Choices.cpp", code);
            _currentChoiceOffset += 9;

            return offset;
        }

        internal int addMultipleAnswerChoice(List<Choice> choiceList, XmlMultipleAnswerProximityCriterionEnum proximityCriterion)
        {
            string choiceText = choiceList[0].AttributeValue.Value.getText(_languageId);
            string comment = choiceList[0].Comment.getText(_languageId);
            string mapId = choiceList[0].Element.XmlElement.mapId == null ? "" : choiceList[0].Element.XmlElement.mapId.Substring(2);

            List<double> pointCriterionValues = new List<double>();

            if (proximityCriterion != XmlMultipleAnswerProximityCriterionEnum.NONE)
            {
                foreach (Choice choice in choiceList)
                {
                    pointCriterionValues.Add(choice.Element.GeoPoint.X);
                    pointCriterionValues.Add(choice.Element.GeoPoint.Y);
                    pointCriterionValues.Add(choice.Element.GeoPoint.Z);
                }
            }

            int offset = _currentChoiceOffset;

            int choiceTextOffset = getStringOffset(choiceText);
            int commentOffset = getStringOffset(comment);
            int mapIdOffset = getStringOffset(mapId);

            int pointCriterionValuesOffset = 0;
            if (pointCriterionValues.Count != 0) pointCriterionValuesOffset = getDoubleArrayOffset(pointCriterionValues);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},0,0,0,0,{6},{7}", offset == 0 ? "" : ",", _currentChoiceOffset, choiceText,
                choiceTextOffset, commentOffset, mapIdOffset, pointCriterionValues.Count / 3, pointCriterionValuesOffset);

            append("Choices.cpp", code);
            _currentChoiceOffset += 9;

            return offset;
        }

        internal int addSimpleAnswerQuestion(SimpleAnswerQuestion question, XmlSimpleAnswerProximityCriterionEnum proximityCriterion)
        {
            string questionText = question.QuestionText.getText(_languageId);
            string answer = question.Choice.AttributeValue.Value.getText(_languageId);
            string comment = question.Choice.AttributeValue.Comment.getText(_languageId);
            if (comment == null) comment = "";

            string questionMapId = "";
            if (question.QuestionElement != null && question.QuestionElement.XmlElement.mapId != null)
            {
                questionMapId = question.QuestionElement.XmlElement.mapId.Substring(2);
            }

            string answerMapId = "";
            if (question.Choice.Element.XmlElement.mapId != null) answerMapId = question.Choice.Element.XmlElement.mapId.Substring(2);

            string excludedChoice = "";
            if (question.ExcludedChoice != null) excludedChoice = question.ExcludedChoice.AttributeValue.Value.getText(_languageId);

            int proximityCriterionType = 0; // NONE
            double doubleCriterionValue = 0.0;
            string stringCriterionValue = "";
            double pointCriterionValueX = 0.0;
            double pointCriterionValueY = 0.0;
            double pointCriterionValueZ = 0.0;

            if (question.ProximityCriterion == XmlSimpleAnswerProximityCriterionEnum.SORT_KEY)
            {
                proximityCriterionType = 1; // STRING
                stringCriterionValue = question.Choice.Element.XmlElement.sortKey;
            }
            else if (question.ProximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER)
            {
                proximityCriterionType = 2; // NUMBER
                Double? d = question.Choice.AttributeValue.Value.getAsDouble();
                if (d != null) doubleCriterionValue = d.Value;
            }
            else if (question.ProximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                proximityCriterionType = 3; // POINT_3D
                GeoPoint p = question.Choice.Element.GeoPoint;
                pointCriterionValueX = p.X;
                pointCriterionValueY = p.Y;
                pointCriterionValueZ = p.Z;
            }

            int offset = _currentSimpleAnswerQuestionOffset;

            int questionTextOffset = getStringOffset(questionText);
            int answerOffset = getStringOffset(answer);
            int commentOffset = getStringOffset(comment);
            int questionMapIdOffset = getStringOffset(questionMapId);
            int answerMapIdOffset = getStringOffset(answerMapId);
            int excludedChoiceOffset = getStringOffset(excludedChoice);
            int stringCriterionValueOffset = getStringOffset(stringCriterionValue);

            int[] doubleCriterionValueInt = doubleToIntArray(doubleCriterionValue);
            int[] pointCriterionValueXInt = doubleToIntArray(pointCriterionValueX);
            int[] pointCriterionValueYInt = doubleToIntArray(pointCriterionValueY);
            int[] pointCriterionValueZInt = doubleToIntArray(pointCriterionValueZ);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                offset == 0 ? "" : ",", _currentSimpleAnswerQuestionOffset, questionText, questionTextOffset, answerOffset, commentOffset,
                questionMapIdOffset, answerMapIdOffset, excludedChoiceOffset, proximityCriterionType, stringCriterionValueOffset,
                doubleCriterionValueInt[0], doubleCriterionValueInt[1], pointCriterionValueXInt[0], pointCriterionValueXInt[1],
                pointCriterionValueYInt[0], pointCriterionValueYInt[1], pointCriterionValueZInt[0], pointCriterionValueZInt[1]);

            append("SimpleAnswerQuestions.cpp", code);
            _currentSimpleAnswerQuestionOffset += 16;

            return offset;
        }

        internal int addMultipleAnswerQuestion(MultipleAnswerQuestion question, XmlMultipleAnswerProximityCriterionEnum proximityCriterion)
        {
            string questionText = question.QuestionText.getText(_languageId);

            string questionMapId = "";
            if (question.QuestionElement != null && question.QuestionElement.XmlElement.mapId != null)
            {
                questionMapId = question.QuestionElement.XmlElement.mapId.Substring(2);
            }

            List<string> answers = new List<string>();
            List<string> comments = new List<string>();
            List<string> answerMapIds = new List<string>();

            foreach (Choice choice in question.ChoiceList)
            {
                answers.Add(choice.AttributeValue.Value.getText(_languageId));

                if ((question.ChoiceCommentMode == ChoiceCommentModeEnum.ATTRIBUTE_COMMENT)) comments.Add(choice.AttributeValue.Comment.getText(_languageId));
                else comments.Add(choice.Comment.getText(_languageId));

                if (choice.Element.XmlElement.mapId == null) answerMapIds.Add("");
                else answerMapIds.Add(choice.Element.XmlElement.mapId.Substring(2));
            }

            string excludedChoice = "";
            if (question.ExcludedChoice != null) excludedChoice = question.ExcludedChoice.AttributeValue.Value.getText(_languageId);

            int proximityCriterionType = 0; // NONE
            double pointCriterionValueX = 0.0;
            double pointCriterionValueY = 0.0;
            double pointCriterionValueZ = 0.0;

            if (proximityCriterion == XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION)
            {
                proximityCriterionType = 3; // POINT_3D
                List<GeoPoint> pointList = new List<GeoPoint>();
                foreach (Choice choice in question.ChoiceList) pointList.Add(choice.Element.GeoPoint);
                GeoPoint meanGeoPoint = GeoPoint.meanGeoPoint(pointList);
                pointCriterionValueX = meanGeoPoint.X;
                pointCriterionValueY = meanGeoPoint.Y;
                pointCriterionValueZ = meanGeoPoint.Z;
            }

            int offset = _currentMultipleAnswerQuestionOffset;

            int questionTextOffset = getStringOffset(questionText);
            int questionMapIdOffset = getStringOffset(questionMapId);

            int answersOffset = 0;
            int commentsOffset = 0;
            int answerMapIdsOffset = 0;
            if (answers.Count() != 0)
            {
                List<int> stringOffsets = new List<int>();
                foreach (string str in answers) stringOffsets.Add(getStringOffset(str));
                answersOffset = getIntArrayOffset(stringOffsets);

                stringOffsets.Clear();
                foreach (string str in comments) stringOffsets.Add(getStringOffset(str == null ? "" : str));
                commentsOffset = getIntArrayOffset(stringOffsets);

                stringOffsets.Clear();
                foreach (string str in answerMapIds) stringOffsets.Add(getStringOffset(str));
                answerMapIdsOffset = getIntArrayOffset(stringOffsets);
            }

            int excludedChoiceOffset = getStringOffset(excludedChoice);

            int[] pointCriterionValueXInt = doubleToIntArray(pointCriterionValueX);
            int[] pointCriterionValueYInt = doubleToIntArray(pointCriterionValueY);
            int[] pointCriterionValueZInt = doubleToIntArray(pointCriterionValueZ);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                offset == 0 ? "" : ",", _currentMultipleAnswerQuestionOffset, questionText, questionTextOffset, questionMapIdOffset,
                answers.Count(), answersOffset, commentsOffset, answerMapIdsOffset, excludedChoiceOffset, proximityCriterionType,
                pointCriterionValueXInt[0], pointCriterionValueXInt[1], pointCriterionValueYInt[0], pointCriterionValueYInt[1],
                pointCriterionValueZInt[0], pointCriterionValueZInt[1]);

            append("MultipleAnswerQuestions.cpp", code);
            _currentMultipleAnswerQuestionOffset += 14;

            return offset;
        }

        internal int addRelationOrderQuestion(RelationOrderQuestion question)
        {
            string questionText = question.QuestionText.getText(_languageId);

            string mapId = "";
            if (question.QuestionElement.XmlElement.mapId != null) mapId = question.QuestionElement.XmlElement.mapId.Substring(2);

            List<string> choiceTexts = new List<string>();
            List<string> choiceMapIds = new List<string>();
            foreach (Choice choice in question.ChoiceList)
            {
                choiceTexts.Add(choice.AttributeValue.Value.getText(_languageId));
                if (choice.Element.XmlElement.mapId != null) choiceMapIds.Add(choice.Element.XmlElement.mapId.Substring(2));
                else choiceMapIds.Add("");
            }

            int offset = _currentRelationOrderQuestionOffset;

            int questionTextOffset = getStringOffset(questionText);
            int mapIdOffset = getStringOffset(mapId);

            int choiceTextsOffset = 0;
            int choiceMapIdsOffset = 0;
            if (choiceTexts.Count() != 0)
            {
                List<int> stringOffsets = new List<int>();
                foreach (string str in choiceTexts) stringOffsets.Add(getStringOffset(str));
                choiceTextsOffset = getIntArrayOffset(stringOffsets);

                stringOffsets.Clear();
                foreach (string str in choiceMapIds) stringOffsets.Add(getStringOffset(str));
                choiceMapIdsOffset = getIntArrayOffset(stringOffsets);
            }

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7}",
                offset == 0 ? "" : ",", _currentRelationOrderQuestionOffset, questionText, questionTextOffset, mapIdOffset,
                choiceTexts.Count(), choiceTextsOffset, choiceMapIdsOffset);

            append("RelationOrderQuestions.cpp", code);
            _currentRelationOrderQuestionOffset += 5;

            return offset;
        }

        internal int addMapParameters(MapParameters parameters)
        {
            int framingLevel = parameters.FramingLevel;
            int questionParametersOffset = addMapSubParameters(parameters.QuestionParameters);
            int answerParametersOffset = addMapSubParameters(parameters.AnswerParameters);
            int wrongChoiceParametersOffset = addMapSubParameters(parameters.WrongChoiceParameters);
            int allAnswersSelectionMode = 0;
            if (parameters.AnswerSelectionMode != null && parameters.AnswerSelectionMode == XmlAnswerSelectionModeEnum.ALL_ANSWERS)
            {
                allAnswersSelectionMode = 1;
            }

            int offset = _currentMapParametersOffset;

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6}",
                 offset == 0 ? "" : ",", _currentMapParametersOffset, framingLevel, questionParametersOffset,
                 answerParametersOffset, wrongChoiceParametersOffset, allAnswersSelectionMode);

            append("MapParameterss.cpp", code);
            _currentMapParametersOffset += 5;

            return offset;
        }

        private int addMapSubParameters(XmlMapSubParameters subParameters)
        {
            int drawDepth = 0;
            string categorySelectionMode = "";
            List<int> categories = new List<int>();
            int framingMode = 0;

            if (subParameters != null)
            {
                drawDepth = Int32.Parse(subParameters.drawDepth);
                categorySelectionMode = subParameters.categorySelectionMode.ToString();

                if (subParameters.category != null)
                {
                    foreach (XmlMapCategory category in subParameters.category)
                    {
                        categories.Add(Int32.Parse(category.categoryIndexInMapConfigFile));
                    }
                }

                if (subParameters.framingMode == XmlFramingModeEnum.ONLY_MAIN_ELEMENT) framingMode = 1;
                else if (subParameters.framingMode == XmlFramingModeEnum.ALL_LINKED_ELEMENTS) framingMode = 2;
            }

            int offset = _currentMapSubParametersOffset;

            int categorySelectionModeOffset = getStringOffset(categorySelectionMode);

            int categoriesOffset = 0;
            if (categories.Count() != 0)
            {
                categoriesOffset = getIntArrayOffset(categories);
            }

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6}",
                 offset == 0 ? "" : ",", _currentMapSubParametersOffset, drawDepth,
                 categorySelectionModeOffset, categories.Count(), categoriesOffset, framingMode);

            append("MapSubParameterss.cpp", code);
            _currentMapSubParametersOffset += 5;

            return offset;
        }

        internal void addSimpleAnswerCategory(int weightIndex, int mapParametersOffset, List<int> questionList, List<int> choiceList,
                                              double distribParameterCorrection, XmlSimpleAnswerProximityCriterionEnum proximityCriterion)
        {
            int proximityCriterionType = 0; // NONE
            if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.SORT_KEY) proximityCriterionType = 1; // STRING
            else if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ATTRIBUTE_VALUE_AS_NUMBER) proximityCriterionType = 2; // NUMBER
            else if (proximityCriterion == XmlSimpleAnswerProximityCriterionEnum.ELEMENT_LOCATION) proximityCriterionType = 3; // POINT_3D

            int offset = _currentSimpleAnswerCategoryOffset;

            int questionListOffset = 0;
            if (questionList.Count() != 0) questionListOffset = getIntArrayOffset(questionList);

            int choiceListOffset = 0;
            if (choiceList.Count() != 0) choiceListOffset = getIntArrayOffset(choiceList);

            int[] distribParameterCorrectionInt = doubleToIntArray(distribParameterCorrection);

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10}", offset == 0 ? "" : ",", _currentSimpleAnswerCategoryOffset,
                weightIndex, mapParametersOffset, questionList.Count, questionListOffset, choiceList.Count, choiceListOffset,
                distribParameterCorrectionInt[0], distribParameterCorrectionInt[1], proximityCriterionType);

            append("SimpleAnswerCategories.cpp", code);
            _currentSimpleAnswerCategoryOffset += 9;

            _simpleAnswerCategoryOffsets.Add(offset);
        }

        internal void addMultipleAnswerCategory(int weightIndex, int mapParametersOffset, List<int> questionList, List<int> choiceList,
                                                double distribParameterCorrection, XmlMultipleAnswerProximityCriterionEnum proximityCriterion)
        {
            int proximityCriterionType = 0; // NONE
            if (proximityCriterion == XmlMultipleAnswerProximityCriterionEnum.ELEMENT_LOCATION) proximityCriterionType = 3; // POINT_3D

            int offset = _currentMultipleAnswerCategoryOffset;

            int questionListOffset = 0;
            if (questionList.Count() != 0) questionListOffset = getIntArrayOffset(questionList);

            int choiceListOffset = 0;
            if (choiceList.Count() != 0) choiceListOffset = getIntArrayOffset(choiceList);

            int[] distribParameterCorrectionInt = doubleToIntArray(distribParameterCorrection);

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10}", offset == 0 ? "" : ",", _currentMultipleAnswerCategoryOffset,
                weightIndex, mapParametersOffset, questionList.Count, questionListOffset, choiceList.Count, choiceListOffset,
                distribParameterCorrectionInt[0], distribParameterCorrectionInt[1], proximityCriterionType);

            append("MultipleAnswerCategories.cpp", code);
            _currentMultipleAnswerCategoryOffset += 9;

            _multipleAnswerCategoryOffsets.Add(offset);
        }

        internal void addRelationOrderCategory(int weightIndex, int mapParametersOffset, List<int> questionList, double distribParameterCorrection)
        {
            int offset = _currentRelationOrderCategoryOffset;

            int questionListOffset = 0;
            if (questionList.Count() != 0) questionListOffset = getIntArrayOffset(questionList);

            int[] distribParameterCorrectionInt = doubleToIntArray(distribParameterCorrection);

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7}", offset == 0 ? "" : ",", _currentRelationOrderCategoryOffset,
                weightIndex, mapParametersOffset, questionList.Count, questionListOffset,
                distribParameterCorrectionInt[0], distribParameterCorrectionInt[1]);

            append("RelationOrderCategories.cpp", code);
            _currentRelationOrderCategoryOffset += 6;

            _relationOrderCategoryOffsets.Add(offset);
        }

        internal void addAttributeOrderCategory(Text questionText, int weightIndex, int mapParametersOffset, List<AttributeOrderElement> elementList,
            int maxIndex, double distribParameterCorrection, XmlAttributeOrderModeEnum mode, XmlNumericalAttributeType numericalAttributeType, string valueFormat)
        {
            string questionStr = questionText.getText(_languageId);

            List<string> choiceTexts = new List<string>();
            List<string> choiceComments = new List<string>();
            List<int> choiceMinIndexes = new List<int>();
            List<string> choiceMapIds = new List<string>();

            foreach (AttributeOrderElement element in elementList)
            {
                choiceTexts.Add(element.Element.Name.getText(_languageId));

                double value = mode == XmlAttributeOrderModeEnum.LOWEST ? element.AttributeValue : -element.AttributeValue;
                string comment = String.Format("{0}{1}", value.ToString(valueFormat, CultureInfo.CreateSpecificCulture("en-US")), numericalAttributeType.unit);
                choiceComments.Add(comment);

                choiceMinIndexes.Add(element.MinAnswerIndex);

                choiceMapIds.Add(element.Element.XmlElement.mapId == null ? "" : element.Element.XmlElement.mapId.Substring(2));
            }

            int offset = _currentAttributeOrderCategoryOffset;

            int questionOffset = getStringOffset(questionStr);

            int choiceTextsOffset = 0;
            int choiceCommentsOffset = 0;
            int choiceMinIndexesOffset = 0;
            int choiceMapIdsOffset = 0;
            if (choiceTexts.Count() != 0)
            {
                List<int> stringOffsets = new List<int>();
                foreach (string str in choiceTexts) stringOffsets.Add(getStringOffset(str));
                choiceTextsOffset = getIntArrayOffset(stringOffsets);

                stringOffsets.Clear();
                foreach (string str in choiceComments) stringOffsets.Add(getStringOffset(str));
                choiceCommentsOffset = getIntArrayOffset(stringOffsets);

                choiceMinIndexesOffset = getIntArrayOffset(choiceMinIndexes);

                stringOffsets.Clear();
                foreach (string str in choiceMapIds) stringOffsets.Add(getStringOffset(str));
                choiceMapIdsOffset = getIntArrayOffset(stringOffsets);
            }

            int[] distribParameterCorrectionInt = doubleToIntArray(distribParameterCorrection);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                offset == 0 ? "" : ",", _currentAttributeOrderCategoryOffset, questionStr,
                weightIndex, mapParametersOffset, questionOffset, choiceTexts.Count(), choiceTextsOffset, choiceCommentsOffset,
                choiceMinIndexesOffset, choiceMapIdsOffset, distribParameterCorrectionInt[0], distribParameterCorrectionInt[1], maxIndex);

            append("AttributeOrderCategories.cpp", code);
            _currentAttributeOrderCategoryOffset += 11;

            _attributeOrderCategoryOffsets.Add(offset);
        }

        private int getDoubleArrayOffset(IEnumerable<double> values)
        {
            int offset = _currentDoubleArrayOffset;

            string code = String.Format(",\n// {0}\n{1}", offset, String.Join(",", values.Select(v => v.ToString(CultureInfo.CreateSpecificCulture("en-US")))));
            append("DoubleArrays.cpp", code);
            _currentDoubleArrayOffset += values.Count();

            return offset;
        }

        private int getIntArrayOffset(IEnumerable<int> values)
        {
            int offset = _currentIntArrayOffset;

            string code = String.Format(",\n// {0}\n{1}", offset, String.Join(",", values));
            append("IntArrays.cpp", code);
            _currentIntArrayOffset += values.Count();

            return offset;
        }

        private int[] doubleToIntArray(double d)
        {
            byte[] b = BitConverter.GetBytes(d);
            int[] r = new int[2];
            r[0] = (((int)b[3]) << 24) | (((int)b[2]) << 16) | (((int)b[1]) << 8) | ((int)b[0]);
            r[1] = (((int)b[7]) << 24) | (((int)b[6]) << 16) | (((int)b[5]) << 8) | ((int)b[4]);
            return r;
        }

        private void append(string fileName, string text)
        {
            string path = String.Format("{0}/{1}", _dirName, fileName);

            StreamWriter file;
            if (!_streamWriterDictionary.TryGetValue(path, out file))
            {
                file = new StreamWriter(path, true);
                _streamWriterDictionary.Add(path, file);
            }

            file.Write(text);
        }
    }
}
    
