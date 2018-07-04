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
    class PointMapElement : MapElement
    {
        private KmlFileData _kmlFile = null;
        private readonly ItemId _itemId = new ItemId();

        internal PointMapElement(String id, MapData mapData, XmlName[] name, XmlName[] shortName, double importance, List<string> lookIds, string categoryId) :
            base(id, mapData, name, shortName, importance, lookIds, categoryId) { }

        protected override bool allowMultiline() { return false; }
        internal ItemId ItemId { get { return _itemId; } }

        internal override int addKmlFile(String path)
        {
            KmlFileData data = KmlFileData.getData(path);

            if (data.Type == KmlFileTypeEnum.LINE)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add line file '{0}' to point element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.POLYGON)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add polygon file '{0}' to point element '{1}'", path, Id));
                return -1;
            }
            else if (data.Type == KmlFileTypeEnum.POINT)
            {
                if (_kmlFile != null)
                {
                    MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not add several files ('{0}' and '{1}') to point element '{2}'", _kmlFile.Path, path, Id));
                    return -1;
                }
                else
                {
                    _kmlFile = data;
                }
            }

            return 0;
        }

        internal GeoPoint Point
        {
            get
            {
                return _kmlFile.PointList[0];
            }
        }

        internal override int formParts1()
        {
            if (_kmlFile == null)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("No KML file associated to element '{0}'", Id));
                return -1;
            }

            return 0;
        }

        internal override int formParts2()
        {
            return 0;
        }

        internal override int generateCode(CodeGenerator codeGenerator)
        {
            codeGenerator.addPointElement(this, _mapData.XmlMapData.parameters.projection);

            return 0;
        }
    }
}
