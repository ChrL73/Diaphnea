using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schemas;

namespace QuestionInstantiation
{
    abstract class Category
    {
        private readonly Int32 _weightIndex;
        private readonly string _questionNameInLog;
        private readonly QuizData _quizData;
        private readonly MapParameters _mapParameters;

        protected Category(int weightIndex, string questionNameInLog, QuizData quizData, MapParameters mapParameters)
        {
            _weightIndex = weightIndex;
            _questionNameInLog = questionNameInLog;
            _quizData = quizData;
            _mapParameters = mapParameters;
        }

        internal MapParameters MapParameters
        {
            get { return _mapParameters; }
        }

        protected Int32 WeightIndex
        {
            get { return _weightIndex; }
        }

        protected string QuestionNameInLog
        {
            get { return _questionNameInLog; }
        }

        protected QuizData QuizData
        {
            get { return _quizData; }
        }

        abstract internal int generateCode(List<CodeGenerator> codeGeneratorList);
    }
}
