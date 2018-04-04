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

        private readonly Dictionary<string, int> _stringDictionary = new Dictionary<string, int>();

        internal CodeGenerator(string dirName, string languageId)
        {
            _dirName = dirName;
            _languageId = languageId;
            _currentStringOffset = 1;
            _currentChoiceOffset = 0;
            _currentIntArrayOffset = 1;
            _currentDoubleArrayOffset = 3;

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
        }

        internal void close()
        {
            append("Choices.cpp", "\n};\n}\n");
            append("DoubleArrays.cpp", "\n};\n}\n");
            append("IntArrays.cpp", "\n};\n}\n");
            append("Strings.cpp", "\n};\n}\n");
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

        private int getDoubleArrayOffset(IEnumerable<double> values)
        {
            int offset = _currentDoubleArrayOffset;

            string code = String.Format(",\n{0}", String.Join(",", values.Select(v => v.ToString(CultureInfo.CreateSpecificCulture("en-US")))));
            append("DoubleArrays.cpp", code);
            _currentDoubleArrayOffset += values.Count();

            return offset;
        }

        private int getIntArrayOffset(IEnumerable<int> values)
        {
            int offset = _currentIntArrayOffset;

            string code = String.Format(",\n{0}", String.Join(",", values));
            append("IntArrays.cpp", code);
            _currentIntArrayOffset += values.Count();

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
    