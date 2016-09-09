using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CleanKml
{
    class CleanKmlMain
    {
        static String _logFileName = "CleanKml.log";
        static String _pointTemplatePath;
        static String _lineTemplatePath;
        static String _polygonTemplatePath;
        static List<String> fileList = new List<String>();
        static List<String> _cleanedFiles = new List<String>();
        static CultureInfo _cultureInfo = new CultureInfo("en-US");
        static bool _recleanAll = false;

        static void Main(String[] args)
        {
            if (args.Length < 1 || (args[0] == "-a" && args.Length < 2))
            {
                Console.WriteLine("usage: CleanKml.exe [-a] path");
            }
            else
            {
                if (File.Exists(_logFileName)) File.Delete(_logFileName);

                if (args[0] == "-a")
                {
                    _recleanAll = true;
                    browse(args[1]);
                }
                else
                {
                    browse(args[0]);
                }

                if (_pointTemplatePath == null)
                {
                    logMessage("!!! Point template not found");
                    Console.WriteLine("Point template not found");
                }
                if (_lineTemplatePath == null)
                {
                    logMessage("!!! Line template not found");
                    Console.WriteLine("Line template not found");
                }
                if (_polygonTemplatePath == null)
                {
                    logMessage("!!! Polygon template not found");
                    Console.WriteLine("Polygon template not found");
                }

                if (_pointTemplatePath != null && _lineTemplatePath != null && _polygonTemplatePath != null)
                {
                    foreach (String path in fileList) cleanFile(path);
                }

                Console.WriteLine();
                logMessage("");
                int n = _cleanedFiles.Count;
                String msg = String.Format("{0} file{1} cleaned{2}", n, n > 1 ? "s" : "", n > 0 ? ":" : "");
                logMessage(msg);
                Console.WriteLine(msg + " (See detail in " + _logFileName + ")");
                foreach (String file in _cleanedFiles) logMessage(file);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void browse(String path)
        {
            FileAttributes attr;

            try
            {
                attr = File.GetAttributes(path);
            }
            catch (FileNotFoundException)
            {
                logMessage("!!! " + path + " not found");
                Console.WriteLine(path + " not found");
                return;
            }

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                logMessage("Browsing " + path);
                List<String> entries = new List<String>(Directory.EnumerateFileSystemEntries(path));
                foreach (var entry in entries) browse(entry);
            }
            else
            {
                String fileName = Path.GetFileName(path);
                if (String.Equals(fileName, "pointtemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage("Point template: " + path);
                    _pointTemplatePath = path;
                }
                else if (String.Equals(fileName, "linetemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage("Line template: " + path);
                    _lineTemplatePath = path;
                }
                else if (String.Equals(fileName, "polygontemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage("Polygon template: " + path);
                    _polygonTemplatePath = path;
                }
                else if (String.Equals(Path.GetExtension(path), ".kml", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage("Adding " + path);
                    fileList.Add(path);                
                }
                else
                {
                    logMessage("Extension is not '.kml': " + path + " ignored");
                }
            }
        }

        static void cleanFile(String path)
        {
            XmlDocument inputDocument = new XmlDocument();
            try
            {
                inputDocument.Load(path);
            }
            catch (Exception)
            {
                logMessage("!!! Invalid file, " + path + " ignored");
                Console.WriteLine("Invalid file, " + path + " ignored");
                return;
            }

            XmlNode rootNode = inputDocument.DocumentElement;
            if (rootNode.Attributes.Count == 0 && !_recleanAll)
            {
                logMessage(path + " already cleaned, file ignored");
                return;
            }

            XmlNodeList inputCoordinatesList = inputDocument.GetElementsByTagName("coordinates");
            if (inputCoordinatesList.Count == 0)
            {
                logMessage("!!! No 'coordinates' tags found, file " + path + " ignored");
                Console.WriteLine("No 'coordinates' tags found, file " + path + " ignored");
                return;
            }

            if (inputCoordinatesList.Count > 1)
            {
                logMessage("!!! More than one 'coordinates' tags in file " + path + ". Only the first one is taken into account");
                Console.WriteLine("More than one 'coordinates' tags in file " + path + ". Only the first one is taken into account");
            }

            String templatePath = null;
            XmlNode inputCoodinatesNode = inputCoordinatesList.Item(0);
            XmlNode parent = inputCoodinatesNode.ParentNode;
            while (parent != null)
            {
                if (parent.Name == "Point")
                {
                    templatePath = _pointTemplatePath;
                    break;
                }
                else if (parent.Name == "LineString")
                {
                    templatePath = _lineTemplatePath;
                    break;
                }
                else if (parent.Name == "Polygon")
                {
                    templatePath = _polygonTemplatePath;
                    break;
                }
                parent = parent.ParentNode;
            }

            if (templatePath == null)
            {
                logMessage("!!! 'coordinates' tag is not incuded in either 'Point' or 'LineString' or 'Polygon' tag. " + path + " ignored");
                Console.WriteLine("'coordinates' tag is not incuded in either 'Point' or 'LineString' or 'Polygon' tag. " + path + " ignored");
                return;
            }

            XmlDocument outputDocument = new XmlDocument();
            outputDocument.PreserveWhitespace = true; // Necessary to set 'indentationPattern' below  
            try
            {
                outputDocument.Load(templatePath);
            }
            catch (Exception)
            {
                logMessage("!!! Invalid template file " + templatePath + ". File" + path + " ignored");
                Console.WriteLine("Invalid template file " + templatePath + ". File" + path + " ignored");
                return;
            }

            XmlNodeList outputCoordinatesList = outputDocument.GetElementsByTagName("coordinates");
            if (outputCoordinatesList.Count == 0)
            {
                logMessage("!!! No 'coordinates' tags found, file " + path + " ignored");
                Console.WriteLine("No 'coordinates' tags found, file " + path + " ignored");
                return;
            }

            if (outputCoordinatesList.Count > 1)
            {
                logMessage("!!! More than one 'coordinates' tags in file " + path + ". Only the first one is taken into account");
                Console.WriteLine("More than one 'coordinates' tags in file " + path + ". Only the first one is taken into account");
            }

            XmlNode outputCoodinatesNode = outputCoordinatesList.Item(0);
            String indentationPattern = outputCoodinatesNode.InnerText;
            if (!indentationPattern.Contains("\n")) indentationPattern = "\n";

            String coordinates = inputCoodinatesNode.FirstChild.Value;
            String[] pointArray = coordinates.Split(' ', '\n', '\t');

            StringBuilder newCoordinates = new StringBuilder();
            foreach (String pointStr in pointArray)
            {
                if (pointStr.Length != 0)
                {
                    newCoordinates.Append("   ");

                    String[] coordinateArray = pointStr.Split(',');
                    double lon;
                    if (!Double.TryParse(coordinateArray[0], NumberStyles.Number | NumberStyles.AllowExponent, _cultureInfo, out lon))
                    {
                        logMessage("!!! Invalid file, can't convert " + coordinateArray[0] + " to double, " + path + " ignored");
                        Console.WriteLine("Invalid file, can't convert " + coordinateArray[0] + " to double, " + path + " ignored");
                        return;
                    }
                    newCoordinates.Append(lon.ToString("#.######", _cultureInfo));
                    newCoordinates.Append(",");

                    double lat;
                    if (!Double.TryParse(coordinateArray[1], NumberStyles.Number | NumberStyles.AllowExponent, _cultureInfo, out lat))
                    {
                        logMessage("!!! Invalid file, can't convert " + coordinateArray[1] + " to double, " + path + " ignored");
                        Console.WriteLine("Invalid file, can't convert " + coordinateArray[1] + " to double, " + path + " ignored");
                        return;
                    }
                    newCoordinates.Append(lat.ToString("#.######", _cultureInfo));
                    newCoordinates.Append(indentationPattern);
                }
            }

            XmlText xmlText = outputDocument.CreateTextNode(newCoordinates.ToString());
            outputCoodinatesNode.AppendChild(xmlText);
            outputDocument.Save(path);
            _cleanedFiles.Add(path);
            Console.WriteLine(path + " cleaned");
        }

        static void logMessage(String message)
        {
            using (StreamWriter file = new StreamWriter(_logFileName, true))
            {
                file.WriteLine(message);
            }
        }
    }
}
