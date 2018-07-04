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

namespace QuestionInstantiation
{
    class RelationOrderCategory : Category
    {
        private readonly List<RelationOrderQuestion> _questionList = new List<RelationOrderQuestion>();
        private readonly double _distribParameterCorrection;

        internal RelationOrderCategory(int weightIndex, string questionNameInLog, QuizData quizData, double distribParameterCorrection, MapParameters mapParameters)
            : base(weightIndex, questionNameInLog, quizData, mapParameters)
        {
            _distribParameterCorrection = distribParameterCorrection;
        }

        internal void addQuestion(RelationOrderQuestion question)
        {
            _questionList.Add(question);
        }

        internal int QuestionCount
        {
            get { return _questionList.Count; }
        }

        internal override int generateCode(List<CodeGenerator> codeGeneratorList)
        {
            foreach (CodeGenerator codeGenerator in codeGeneratorList)
            {
                List<int> questionList = new List<int>();

                foreach (RelationOrderQuestion question in _questionList)
                {
                    questionList.Add(codeGenerator.addRelationOrderQuestion(question));
                }

                int mapParamOffset = codeGenerator.addMapParameters(MapParameters);

                codeGenerator.addRelationOrderCategory(WeightIndex, mapParamOffset, questionList, _distribParameterCorrection);
            }

            return 0;
        }
    }
}
