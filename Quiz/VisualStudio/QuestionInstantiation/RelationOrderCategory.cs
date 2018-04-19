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
