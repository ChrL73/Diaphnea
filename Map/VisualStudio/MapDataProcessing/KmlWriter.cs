using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
