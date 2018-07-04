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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Schemas;

namespace MapDataProcessing
{
    class KmlWriter
    {
        static private CultureInfo _cultureInfo = new CultureInfo("en-US");

        static internal int write(List<GeoPoint> pointList, KmlFileTypeEnum type, String subDir, String fileName, XmlResolution resolution)
        {
            String dir = resolution.outputDir + "/" + subDir;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            String templateResource = null;
            if (type == KmlFileTypeEnum.LINE) templateResource = "MapDataProcessing.LineTemplate.kml";
            else if (type == KmlFileTypeEnum.POLYGON) templateResource = "MapDataProcessing.PolygonTemplate.kml";
            else if (type == KmlFileTypeEnum.POINT) templateResource = "MapDataProcessing.PointTemplate.kml";

            if (templateResource == null)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Can not find KML template for type '{0}' to write file '{1}'", type, fileName));
                return -1;
            }
            else
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                XmlDocument document = new XmlDocument();
                document.PreserveWhitespace = true;

                using (Stream stream = assembly.GetManifestResourceStream(templateResource))
                {
                    document.Load(stream);
                }

                XmlNodeList coordinatesList = document.GetElementsByTagName("coordinates");
                XmlNode coordinatesNode = coordinatesList.Item(0);
                String indentationPattern = coordinatesNode.InnerText;
                if (!indentationPattern.Contains("\n")) indentationPattern = "\n";

                StringBuilder coordinates = new StringBuilder();
                foreach (GeoPoint point in pointList)
                {             
                    coordinates.Append("   ");
                    coordinates.Append(point.Longitude.ToString("#.######", _cultureInfo));
                    coordinates.Append(",");
                    coordinates.Append(point.Latitude.ToString("#.######", _cultureInfo));
                    coordinates.Append(indentationPattern);      
                }

                XmlText xmlText = document.CreateTextNode(coordinates.ToString());
                coordinatesNode.AppendChild(xmlText);
                document.Save(dir + "/" + fileName);
            }

            return 0;
        }
    }
}
