using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Category
    {
        private readonly Int32 _weightIndex;

        protected Category(int weightIndex)
        {
            _weightIndex = weightIndex;
        }

        internal Int32 WeightIndex
        {
            get { return _weightIndex; }
        }
    }
}
