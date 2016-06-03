using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class Level
    {
        private XmlLevel _xmlLevel;
        private Dictionary<string, Element> _elementDictionary = new Dictionary<string, Element>();
	    private Dictionary<ElementType, List<Element>> _elementByTypeDictionary = new Dictionary<ElementType, List<Element>>();
        private List<Category> _categoryList = new List<Category>();

        internal int initialize(QuizData quizData, XmlLevel xmlLevel)
        {
            _xmlLevel = xmlLevel;

            return 0;
        }
    }
}
