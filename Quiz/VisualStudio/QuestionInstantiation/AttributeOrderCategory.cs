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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    class AttributeOrderCategory : Category
    {
        private readonly SortedDictionary<double, List<Element>> _choiceDictionary = new SortedDictionary<double, List<Element>>();
        private readonly List<AttributeOrderElement> _elementList = new List<AttributeOrderElement>();
        private readonly XmlNumericalAttributeType _numericalAttributeType;
        private readonly Text _questionText;
        private readonly XmlAttributeOrderModeEnum _mode;
        private readonly double _distribParameterCorrection;
        private readonly string _valueFormat;
        private int _maxIndex;

        internal AttributeOrderCategory (int weightIndex, string questionNameInLog, QuizData quizData, XmlNumericalAttributeType numericalAttributeType,
                                         Text questionText, XmlAttributeOrderModeEnum mode, double distribParameterCorrection, MapParameters mapParameters)
            : base(weightIndex, questionNameInLog, quizData, mapParameters)
        {
            _numericalAttributeType = numericalAttributeType;
            _questionText = questionText;
            _mode = mode;
            _distribParameterCorrection = distribParameterCorrection;
            _maxIndex = -1;

            _valueFormat = "0.";
            int i, n = Int32.Parse(_numericalAttributeType.displayPrecision);
            for (i = 0; i < n; ++i) _valueFormat += "#";
        }

        internal int ChoiceCount { get { return _choiceDictionary.Count; } }

        internal void addAnswer (double value, Element element)
        {
            if (_mode == XmlAttributeOrderModeEnum.GREATEST) value = -value;
            if (!_choiceDictionary.ContainsKey(value)) _choiceDictionary.Add(value, new List<Element>());
            _choiceDictionary[value].Add(element);
        }

        internal int setElementVector(int choiceCount)
        {
            foreach (KeyValuePair<double, List<Element>> pair in _choiceDictionary)
            {
                foreach (Element answerElement in pair.Value)
                {
                    AttributeOrderElement element = new AttributeOrderElement(pair.Key, answerElement);
                    _elementList.Add(element);
                }
            }

            int i, n = _elementList.Count;
            int minIndex = n - 1;
            bool minIndexExists = false;
            for (i = n - 2; i >= 0; --i)
            {
                int j;
                for (j = i + 1; j <= minIndex; j++)
                {
                    double d = _elementList[i].AttributeValue - _elementList[j].AttributeValue;
                    if (d < 0) d = -d;
                    if (d > _numericalAttributeType.ambiguityThreshold)
                    {
                        minIndex = j;
                        minIndexExists = true;
                        break;
                    }
                }
                if (minIndexExists)
                {
                    _elementList[i].MinAnswerIndex = minIndex;
                    if (minIndex + choiceCount <= n + 1) ++_maxIndex;
                }
            }

            if (_maxIndex < 0) return -1;
            return 0;
        }

        internal override int generateCode(List<CodeGenerator> codeGeneratorList)
        {
            foreach (CodeGenerator codeGenerator in codeGeneratorList)
            {
                int mapParamOffset = codeGenerator.addMapParameters(MapParameters);
                codeGenerator.addAttributeOrderCategory(_questionText, WeightIndex, mapParamOffset, _elementList, _maxIndex,
                                                        _distribParameterCorrection, _mode, _numericalAttributeType, _valueFormat);
            }

            return 0;
        }
    }
}
