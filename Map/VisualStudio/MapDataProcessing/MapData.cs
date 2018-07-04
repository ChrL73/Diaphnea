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
using Schemas;

namespace MapDataProcessing
{
    class MapData
    {
        private readonly string _dataFileName;
        private readonly XmlMapData _xmlMapData;
        private readonly Dictionary<string, Look> _lookDictionary = new Dictionary<string, Look>();
        private readonly List<Look> _lookList = new List<Look>();
        private readonly Dictionary<string, Category> _categoryDictionary = new Dictionary<string, Category>();
        private readonly List<Category> _categoryList = new List<Category>();

        private readonly Dictionary<XmlResolution, LineSuperposer> _lineSuperposerDictionary = new Dictionary<XmlResolution, LineSuperposer>();

        internal MapData(string dataFileName, XmlMapData xmlMapData)
        {
            _dataFileName = dataFileName;
            _xmlMapData = xmlMapData;
            foreach (XmlPolygonLook1 polygonLook1 in _xmlMapData.lookList.polygonLookList1)
            {
                Look look = new PolygonLook(polygonLook1, _xmlMapData.lookList.nameSuffixes);
                _lookDictionary.Add(polygonLook1.id, look);
                _lookList.Add(look);
            }

            foreach (XmlPolygonLook2 polygonLook2 in _xmlMapData.lookList.polygonLookList2)
            {
                Look look = new PolygonLook(polygonLook2, _xmlMapData.lookList.nameSuffixes);
                _lookDictionary.Add(polygonLook2.id, look);
                _lookList.Add(look);
            }

            foreach (XmlLineLook lineLook in _xmlMapData.lookList.lineLookList)
            {
                Look look = new LineLook(lineLook, _xmlMapData.lookList.nameSuffixes);
                _lookDictionary.Add(lineLook.id, look);
                _lookList.Add(look);
            }

            foreach (XmlPointLook pointLook in _xmlMapData.lookList.pointLookList)
            {
                Look look = new PointLook(pointLook, _xmlMapData.lookList.nameSuffixes);
                _lookDictionary.Add(pointLook.id, look);
                _lookList.Add(look);
            }

            foreach (XmlCategory xmlCategory in _xmlMapData.categoryList)
            {
                Category category = new Category(xmlCategory);
                _categoryDictionary.Add(xmlCategory.id, category);
                _categoryList.Add(category);
            }

            foreach (XmlResolution xmlResolution in _xmlMapData.resolutionList) _lineSuperposerDictionary.Add(xmlResolution, new LineSuperposer());
        }

        internal XmlMapData XmlMapData
        {
            get { return _xmlMapData; }
        }

        internal string DataFileName
        {
            get { return _dataFileName; }
        }

        internal Look getLook(string id)
        {
            Look look;
            if (_lookDictionary.TryGetValue(id, out look)) return look;
            return null;
        }

        internal List<Look> LookList { get { return _lookList; } }

        internal Category getCategory(string id)
        {
            Category category;
            if (_categoryDictionary.TryGetValue(id, out category)) return category;
            return null;
        }

        internal List<Category> CategoryList { get { return _categoryList; } }

        internal  Dictionary<XmlResolution, LineSuperposer> LineSuperposerDictionary { get { return _lineSuperposerDictionary; } }
    }
}
