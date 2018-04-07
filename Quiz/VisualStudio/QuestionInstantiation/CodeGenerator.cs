using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace QuestionInstantiation
{
    class CodeGenerator
    {
        private readonly string _dirName;
        private readonly string _languageId;

        private int _currentStringOffset;
        private int _currentChoiceOffset;
        private int _currentIntArrayOffset;
        private int _currentDoubleArrayOffset;
        private int _currentSimpleAnswerQuestionOffset;
        private int _currentMapParametersOffset;
        private int _currentMapSubParametersOffset;
        private int _currentSimpleAnswerCategoryOffset;

        private readonly List<int> _simpleAnswerQuestionCategoryOffsets = new List<int>();
        private readonly List<int> _multipleAnswerQuestionCategoryOffsets = new List<int>();
        private readonly List<int> _relationOrderQuestionCategoryOffsets = new List<int>();
        private readonly List<int> _attributeOrderQuestionCategoryOffsets = new List<int>();

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
        }

        internal void close(int questionCount, int weightSum, double distribParameter, int choiceCount)
        {
            int simpleAnswerQuestionCategoryOffsets = 0;
            if (_simpleAnswerQuestionCategoryOffsets.Count() != 0)
            {
                simpleAnswerQuestionCategoryOffsets = getIntArrayOffset(_simpleAnswerQuestionCategoryOffsets);
            }

            int multipleAnswerQuestionCategoryOffsets = 0;
            if (_multipleAnswerQuestionCategoryOffsets.Count() != 0)
            {
                multipleAnswerQuestionCategoryOffsets = getIntArrayOffset(_multipleAnswerQuestionCategoryOffsets);
            }

            int relationOrderQuestionCategoryOffsets = 0;
            if (_relationOrderQuestionCategoryOffsets.Count() != 0)
            {
                relationOrderQuestionCategoryOffsets = getIntArrayOffset(_relationOrderQuestionCategoryOffsets);
            }

            int attributeOrderQuestionCategoryOffsets = 0;
            if (_attributeOrderQuestionCategoryOffsets.Count() != 0)
            {
                attributeOrderQuestionCategoryOffsets = getIntArrayOffset(_attributeOrderQuestionCategoryOffsets);
            }

            int[] distribParameterInt = doubleToIntArray(distribParameter);

            string path = String.Format("{0}/Level_.cpp", _dirName);
            if (File.Exists(path)) File.Delete(path);

            string code = String.Format(
                "namespace produce_questions\n{{\nint level[] =\n{{\n{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}\n}};\n}}\n",
                _simpleAnswerQuestionCategoryOffsets.Count(), simpleAnswerQuestionCategoryOffsets,
                _multipleAnswerQuestionCategoryOffsets.Count(), multipleAnswerQuestionCategoryOffsets,
                _relationOrderQuestionCategoryOffsets.Count(), relationOrderQuestionCategoryOffsets,
                _attributeOrderQuestionCategoryOffsets.Count(), attributeOrderQuestionCategoryOffsets,
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

        internal void addSimpleAnswerCategory(int weightIndex, int mapParametersOffset, List<int>questionList, List<int> choiceList,
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

            _simpleAnswerQuestionCategoryOffsets.Add(offset);
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

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.Write(text);
            }
        }
    }
}
    