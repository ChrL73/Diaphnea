using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionInstantiation
{
    class RelationType
    {
        private readonly string _id;
        private readonly RelationNatureEnum _nature;
        private readonly XmlElementType _startType;
        private readonly XmlElementType _endType;
        private readonly RelationWayEnum _way;
        private readonly XmlCheckSymetryEnum _checkSymetry;
        private readonly string _fullName;

        internal RelationType(string id, RelationNatureEnum nature, XmlElementType startType, XmlElementType endType, RelationWayEnum way, XmlCheckSymetryEnum checkSymetry)
        {
            _id = id;
            _nature = nature;
            _startType = startType;
            _endType = endType;
            _way = way;
            _checkSymetry = checkSymetry;

            if (_way == RelationWayEnum.DIRECT) _fullName = String.Format("{0}, direct", _id);
            else _fullName = String.Format("{0}, inverse", _id);
        }

        internal RelationType ReciprocalType { get; set; }

        internal string Id
        {
            get { return _id; }
        }

        internal RelationNatureEnum Nature
        {
            get { return _nature; }
        }

        internal XmlElementType StartType
        {
            get { return _startType; }
        }

        internal XmlElementType EndType
        {
            get { return _endType; }
        }

        internal RelationWayEnum Way
        {
            get { return _way; }
        }

        internal XmlCheckSymetryEnum CheckSymetry
        {
            get { return _checkSymetry; }
        }

        internal string FullName
        {
            get { return _fullName; }
        }
    }
}
